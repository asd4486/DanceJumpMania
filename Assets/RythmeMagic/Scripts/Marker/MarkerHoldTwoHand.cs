using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace RythhmMagic
{
    public class MarkerHoldTwoHand : MarkerHold
    {
        bool touchLeft;
        bool touchRight;

        protected override void OnTriggerStay(Collider col)
        {
            if (col.gameObject.GetComponent<MarkerController>() != null)
            {
                var controller = col.gameObject.GetComponent<MarkerController>();
                if (controller.CurrentHand.handType == SteamVR_Input_Sources.LeftHand) touchLeft = true;
                else touchRight = true;

                if (touchLeft && touchRight) OnHitMarker();
            }
        }

        protected override void OnTriggerExit(Collider col)
        {
            if (!myCol.enabled) return;

            if (col.gameObject.GetComponent<MarkerController>() != null)
            {
                var controller = col.gameObject.GetComponent<MarkerController>();
                if (controller.CurrentHand.handType == SteamVR_Input_Sources.LeftHand) touchLeft = false;
                else touchRight = false;

                fxTouch.Stop();
                main.BreakCombo();
            }
        }
    }
}