using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace RythhmMagic
{
	public class MarkerHoldTwoHand : MarkerHold
	{
        MarkerController hitedControllerL;
        MarkerController hitedControllerR;

        public override void Init(MusicSheetObject.BeatInfo beat, float beatTime)
		{
			markerLine = GetComponentInChildren<MarkerLine>();
			markerLine.SetMaterial(markerRenderer.material);

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

		protected override void OnTriggerStay(Collider col)
		{
			if (col.gameObject.GetComponent<MarkerController>() != null)
			{
                var controller = col.gameObject.GetComponent<MarkerController>();
                if (controller.CurrentHand.handType == SteamVR_Input_Sources.LeftHand)
					hitedControllerL = controller;
                else
					hitedControllerR = controller;

                if (hitedControllerL != null && hitedControllerR != null)
                {
					controller.TouchMarker();
					OnHitMarker();

					//first hit
					if (!fxTouch.isPlaying)
                    {
                        hitedControllerL.Vibrate();
                        hitedControllerR.Vibrate();
                    }                     
                }
            }
		}

		protected override void OnTriggerExit(Collider col)
		{
			if (!myCol.enabled) return;

			if (col.gameObject.GetComponent<MarkerController>() != null)
			{
				var controller = col.gameObject.GetComponent<MarkerController>();
                if (controller.CurrentHand.handType == SteamVR_Input_Sources.LeftHand) hitedControllerL = null;
                else hitedControllerR = null;

                fxTouch.Stop();
				BreakCombo();
			}
		}
	}
}