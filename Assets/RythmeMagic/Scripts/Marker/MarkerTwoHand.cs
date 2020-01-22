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

		protected override void OnTriggerStay(Collider col)
		{
			if (col.gameObject.GetComponent<MarkerController>() != null)
			{
				var controller = col.gameObject.GetComponent<MarkerController>();
				if (controller.CurrentHand.handType == SteamVR_Input_Sources.LeftHand) touchLeft = true;
				else touchRight = true;

				if (touchLeft && touchRight)
				{
					controller.TouchMarker();
					OnHitMarker();
				}
			}
		}

		protected override void OnTriggerExit(Collider col)
		{
			if (col.gameObject.GetComponent<MarkerController>() != null)
			{
				var controller = col.gameObject.GetComponent<MarkerController>();
				if (controller.CurrentHand.handType == SteamVR_Input_Sources.LeftHand) touchLeft = false;
				else touchRight = false;
			}
		}
	}
}
