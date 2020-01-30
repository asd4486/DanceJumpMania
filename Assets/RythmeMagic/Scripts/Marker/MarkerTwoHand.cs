using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

namespace RythhmMagic
{
	public class MarkerTwoHand : MarkerBase
	{
        MarkerController hitedControllerL;
        MarkerController hitedControllerR;

        protected override void OnTriggerStay(Collider col)
		{
			if (col.gameObject.GetComponent<MarkerController>() != null)
			{
				var controller = col.gameObject.GetComponent<MarkerController>();
				if (controller.CurrentHand.handType == SteamVR_Input_Sources.LeftHand) hitedControllerL = controller;
				else hitedControllerR = controller;

				if (hitedControllerL != null && hitedControllerR != null)
				{
					controller.TouchMarker();
					OnHitMarker();

                    hitedControllerL.Vibrate();
                    hitedControllerR.Vibrate();
				}
			}
		}

		protected override void OnTriggerExit(Collider col)
		{
			if (col.gameObject.GetComponent<MarkerController>() != null)
			{
				var controller = col.gameObject.GetComponent<MarkerController>();
				if (controller.CurrentHand.handType == SteamVR_Input_Sources.LeftHand) hitedControllerL = null;
				else hitedControllerR = null;
			}
		}
	}
}
