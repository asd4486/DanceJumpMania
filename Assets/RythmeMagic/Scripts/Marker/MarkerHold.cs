using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using DG.Tweening;

namespace RythhmMagic
{
    public class MarkerHold : MarkerBase
    {
        MarkerLine markerLine;

        //for moving Marker
        float movePathSpeed;

        float oneSecSpeed;
        float markerDuration;

        public override void Init(MusicSheetObject.BeatItem beat, float beatTime)
        {
            markerLine = GetComponentInChildren<MarkerLine>();

            //create holding road
            if (beat.holdingPos != null)
            {
                List<Vector3> posList = new List<Vector3>();
                posList.Add(Vector3.zero);
                posList.Add(new Vector3(0, 0, 0.01f));

                oneSecSpeed =  GameManager.Instance.markerDistance / GameManager.Instance.MarkerSpeed;
                //get hold duration
                markerDuration = beat.holdingPos[beat.holdingPos.Length - 1].time - beatTime;

                foreach (var p in beat.holdingPos)
                {
                    //get holding road lenght
                    var roadLenght = oneSecSpeed * (p.time - beatTime);
                    var adjustPos = p.pos - beat.startPos;
                    posList.Add(new Vector3(adjustPos.x, adjustPos.y, roadLenght));
                }

                //draw road
                markerLine.GenerateMesh(posList.ToArray());
            }

            base.Init(beat, beatTime);
        }

        protected override void Update()
        {
            if (!startMove || !myCol.enabled) return;

            if (transform.localPosition.z <= 0)
            {
                FollowRoadPath();
            }
        }

        //timer for follow path (0 to 1)
        float pathTimer;
        void FollowRoadPath()
        {
            if (pathTimer >= 1)
            {
                myCol.enabled = false;
                myCol.transform.DOScale(Vector3.zero, 0.1f);
                if(fxTouch.isPlaying) fxTouch.Stop();
                Destroy(gameObject, 0.2f);
                return;
            }

            markerLine.transform.position -= transform.forward * oneSecSpeed * Time.deltaTime;

            pathTimer += Time.deltaTime / markerDuration;
            var pathPos = markerLine.vectexPath.GetPointAtTime(pathTimer, EndOfPathInstruction.Stop);
            myCol.transform.position = new Vector3(pathPos.x, pathPos.y, transform.position.z);

            //transform.rotation = path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
        }

        protected override void OnHitMarker()
        {
            if (!fxTouch.isPlaying) fxTouch.Play();
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.GetComponent<MarkerController>() != null)
                fxTouch.Stop();
        }
    }
}
