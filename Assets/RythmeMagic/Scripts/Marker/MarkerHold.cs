using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using DG.Tweening;

namespace RythhmMagic
{
    public class MarkerHold : MarkerBase
    {
        protected MarkerLine markerLine;
        protected float markerDuration;

        protected float addScoreTimer;
        Coroutine breakComboCoroutine;

        private void Start()
        {
            addScoreTimer = gameMgr.addScoreDelay;
        }

        public override void Init(MusicSheetObject.BeatInfo beat, float beatTime)
        {
            markerLine = GetComponentInChildren<MarkerLine>();

            if (beat.markerType == MarkerType.Default) markerLine.SetMaterial(defaultMat);
            else if (beat.markerType == MarkerType.Trigger) markerLine.SetMaterial(triggerMat);

            //create holding road
            if (beat.posList != null)
            {
                List<Vector3> posList = new List<Vector3>();
                posList.Add(Vector3.zero);
                posList.Add(new Vector3(0, 0, 0.01f));

                //get hold duration
                markerDuration = beat.posList[beat.posList.Count - 1].time - beatTime;

                for (int i = 1; i < beat.posList.Count; i++)
                {
                    var p = beat.posList[i];
                    //get holding road lenght
                    var roadLenght = markerSpeed * (p.time - beatTime);
                    var adjustPos = p.pos - beat.posList[0].pos;
                    posList.Add(new Vector3(adjustPos.x, adjustPos.y, roadLenght));
                }

                //draw road
                markerLine.GenerateLine(posList.ToArray());
            }

            base.Init(beat, beatTime);
        }

        protected override IEnumerator ActiveColCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            myCol.enabled = true;
            BreakCombo();
        }

        void BreakCombo()
        {
            if (breakComboCoroutine != null) StopCoroutine(breakComboCoroutine);
            breakComboCoroutine = StartCoroutine(BreakComboCorou());
        }

        IEnumerator BreakComboCorou()
        {
            yield return new WaitForSeconds(0.1f);
            main.BreakCombo();
            breakComboCoroutine = null;
        }

        protected override void Update()
        {
            if (!startMove) return;

            if (transform.localPosition.z > 0) transform.position -= transform.forward * markerSpeed * Time.deltaTime;
            else FollowRoadPath();
        }

        //timer for follow path (0 to 1)
        float pathTimer;
        void FollowRoadPath()
        {
            if (pathTimer >= 1)
            {
                startMove = myCol.enabled = false;
                markerRenderer.transform.DOScale(Vector3.zero, 0.1f);
                if (fxTouch.isPlaying) fxTouch.Stop();
                Destroy(gameObject, 0.2f);
                return;
            }

            markerLine.transform.position -= transform.forward * markerSpeed * Time.deltaTime;

            pathTimer += Time.deltaTime / markerDuration;
            var pathPos = markerLine.vectexPath.GetPointAtTime(pathTimer, EndOfPathInstruction.Stop);
            markerRenderer.transform.position = new Vector3(pathPos.x, pathPos.y, transform.position.z);

            //transform.rotation = path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
        }

        protected override void OnHitMarker()
        {
            if (!fxTouch.isPlaying) fxTouch.Play();

            addScoreTimer += Time.deltaTime;
            if (addScoreTimer >= gameMgr.addScoreDelay)
            {
                main.AddScore();
                addScoreTimer = 0;

                if (breakComboCoroutine != null) StopCoroutine(breakComboCoroutine);
            }
        }

        protected override void OnTriggerExit(Collider col)
        {
            if (!myCol.enabled) return;

            if (col.gameObject.GetComponent<MarkerController>() != null &&
                col.gameObject.GetComponent<MarkerController>() == hitedController)
            {
                hitedController = null;
                fxTouch.Stop();
                BreakCombo();
            }
        }
    }
}
