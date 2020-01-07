using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

namespace RythhmMagic
{
    public class MarkerTwoHand : MarkerBase
    {
        bool touchLeft;
        bool touchRight;

        public override void Init(MusicSheetObject.BeatInfo beat, float beatTime)
        {
            currentBeat = beat;
            transform.localPosition = new Vector3(beat.posList[0].pos.x, beat.posList[0].pos.y, gameMgr.markerDistance);
            StartCoroutine(ActiveColCoroutine(gameMgr.markerTime - 0.1f));

            startMove = true;
        }

        protected override void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.GetComponent<MarkerController>() != null)
            {
                var controller = collision.gameObject.GetComponent<MarkerController>();
                if (controller.CurrentHand.handType == SteamVR_Input_Sources.LeftHand) touchLeft = true;
                else touchRight = true;

                if (touchLeft && touchRight) OnHitMarker();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.GetComponent<MarkerController>() != null)
            {
                var controller = collision.gameObject.GetComponent<MarkerController>();
                if (controller.CurrentHand.handType == SteamVR_Input_Sources.LeftHand) touchLeft = false;
                else touchRight = false;
            }
        }
    }
}
