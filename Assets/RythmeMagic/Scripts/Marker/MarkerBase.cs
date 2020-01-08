using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RythhmMagic
{
    public class MarkerBase : MonoBehaviour
    {
        protected RythmMagicMain main;
        protected MusicSheetObject.BeatInfo currentBeat;
        protected Collider myCol;
        [SerializeField] protected ParticleSystem fxTouch;
        protected GameManager gameMgr;
        protected float markerSpeed;

        protected bool startMove;

        [SerializeField] protected SpriteRenderer markerRenderer;
        [SerializeField] protected Material defaultMat;
        [SerializeField] protected Material triggerMat;

        // Start is called before the first frame update
        void Awake()
        {
            main = FindObjectOfType<RythmMagicMain>();
            gameMgr = FindObjectOfType<GameManager>();
            markerSpeed = gameMgr.MarkerSpeed;

            myCol = GetComponentInChildren<Collider>();
            myCol.enabled = false;
        }

        public virtual void Init(MusicSheetObject.BeatInfo beat, float beatTime)
        {
            if (beat.markerType == MarkerType.Default) markerRenderer.material = defaultMat;
            else if (beat.markerType == MarkerType.Trigger) markerRenderer.material = triggerMat;

            currentBeat = beat;
            transform.localPosition = new Vector3(beat.posList[0].pos.x, beat.posList[0].pos.y, gameMgr.markerDistance);
            StartCoroutine(ActiveColCoroutine(gameMgr.markerTime - 0.1f));

            startMove = true;
        }

        protected IEnumerator ActiveColCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            myCol.enabled = true;
        }

        protected virtual void Update()
        {
            if (!startMove)
                return;

            transform.localPosition -= Vector3.forward * gameMgr.MarkerSpeed * Time.deltaTime;
            if (transform.localPosition.z <= 0)
            {
                startMove = false;
                StartCoroutine(WaitPlayerHitCoroutine());
            }
        }

        IEnumerator WaitPlayerHitCoroutine()
        {
            yield return new WaitForSeconds(0.15f);
            if (myCol.enabled)
            {
                Destroy(gameObject);
                main.BreakCombo();
            }
        }

        protected virtual void OnHitMarker()
        {
            startMove = myCol.enabled = false;
            markerRenderer.transform.DOScale(Vector3.zero, 0.1f);
            fxTouch.Play();
            main.AddScore();

            Destroy(gameObject, 0.2f);
        }

        protected virtual void OnTriggerStay(Collider col)
        {
            if (col.gameObject.GetComponent<MarkerController>() != null &&
                col.gameObject.GetComponent<MarkerController>().controlMarkerType == currentBeat.markerType)
            {
                OnHitMarker();
            }
        }

        protected virtual void OnTriggerExit(Collider col) { }
    }
}