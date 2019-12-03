using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MarkerController : MonoBehaviour
{
	[SerializeField] Hand currentHand;

	// Update is called once per frame
	void Update()
	{
		transform.localPosition = new Vector2(currentHand.transform.position.x, currentHand.transform.position.y);
	}

}
