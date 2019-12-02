using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythmePingPong
{
	public class ParabolaPongCol : MonoBehaviour
	{
		AIParabolaPong parabolaPong;
		// Start is called before the first frame update
		void Start()
		{
			parabolaPong = GetComponentInParent<AIParabolaPong>();
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.GetComponent<AIRacket>() != null)
			{
				parabolaPong.OnTouchedRacket();
				gameObject.SetActive(false);
			}
		}
	}
}