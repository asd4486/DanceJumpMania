using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
    public class AIMarker : MonoBehaviour
    {
        MarkerLine line;

        MusicSheetObject.BeatItem currentBeat;
        BeatTypes beatType;
        Collider myCol;
        // Start is called before the first frame update
        void Awake()
        {
            myCol = GetComponent<Collider>();
            myCol.enabled = false;

            line = GetComponentInChildren<MarkerLine>();
            //transform.localScale = Vector3.zero;
        }

        public void Init(MusicSheetObject.BeatItem beat, float moveDuration)
        {         
            Vector3[] testPoints = new Vector3[4] { new Vector3(0, 0, 0), new Vector3(1, 0, 2), new Vector3(0, 3, 5), new Vector3(0, 4, 10) };
            line.GenerateMesh(testPoints);

            //beatType = beat.type;

            //transform.localPosition = new Vector3(beat.startPos.x, beat.startPos.y, 25);
            //StartCoroutine(ActiveColCoroutine(moveDuration - 0.1f));

            //DOTween.To(() => transform.localPosition, x => transform.localPosition = x, new Vector3(transform.localPosition.x, transform.localPosition.y, 0),
            //    moveDuration).SetEase(Ease.Linear);

        }

        IEnumerator ActiveColCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            myCol.enabled = true;
        }

        bool isDestroied;
        private void Update()
        {
            //if (isDestroied) return;
            //if (transform.localPosition.z <= 0)
            //{
            //    isDestroied = true;
            //    GetComponent<AudioSource>().Play();
            //    Destroy(gameObject, 0.3f);
            //}
        }

        public void OnTouchMarker()
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.black;
            isDestroied = true;
            GetComponent<AudioSource>().Play();
            Destroy(gameObject, 0.3f);
        }
    }
}