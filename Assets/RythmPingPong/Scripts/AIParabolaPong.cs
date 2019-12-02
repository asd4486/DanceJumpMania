using RythmePingPong.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythmePingPong
{
	public class AIParabolaPong : MonoBehaviour
	{
		ParabolaController parabola;
		AudioSource myAudio;

		[SerializeField] Transform point2;
		[SerializeField] Transform point3;

		[SerializeField] Transform point4;
		[SerializeField] Transform point5;
		[SerializeField] Transform point6;

		float spawnMaxAngle;
		float opposite;
		float opposite2;

		[SerializeField] float adjacentMaxPoint;

		[SerializeField] float oppositeStartPoint;
		[SerializeField] float oppositeEndPoint0;
		[SerializeField] float oppositeEndPoint1;

		// Start is called before the first frame update
		void Awake()
		{
			parabola = GetComponentInChildren<ParabolaController>();
			myAudio = GetComponent<AudioSource>();

			float adjacent = 0 - adjacentMaxPoint;
			opposite = oppositeStartPoint - oppositeEndPoint0;
			opposite2 = oppositeEndPoint0 - oppositeEndPoint1;

			spawnMaxAngle = Mathf.Abs(Mathf.Atan2(opposite + opposite2, adjacent) * (180 / Mathf.PI));
			if (spawnMaxAngle > 90) spawnMaxAngle -= 90;

			//set random angle
			var randAngle = Random.Range(-spawnMaxAngle, spawnMaxAngle);
			var angleC = (90 - Mathf.Abs(randAngle)) * (Mathf.PI / 180);

			var xPos = opposite / Mathf.Tan(angleC) * (randAngle < 0 ? -1 : 1);
			point2.transform.position = new Vector3(xPos / 2, 1.7f, 2.5f);
			point3.transform.position = new Vector3(xPos, 0.8f, oppositeEndPoint0);

			point4.transform.position = new Vector3(xPos, 0.8f, oppositeEndPoint0);

			var xPos2 = opposite2 / Mathf.Tan(angleC) * (randAngle < 0 ? -1 : 1);
			point5.transform.position = new Vector3(xPos + xPos2 / 2, 1.3f, 0.2f);
			point6.transform.position = new Vector3(xPos + xPos2, 1.5f, oppositeEndPoint1);
		}

		public void Init(float duration)
		{
			parabola.duration = duration;
			parabola.onFinishedAction += DestroyObject;
			parabola.onNextRoodAction += Parabola_onNextRoodAction;
			parabola.StartParabola();
		}

		private void Parabola_onNextRoodAction()
		{
			myAudio.Play();
		}

		private void DestroyObject()
		{
			Destroy(gameObject);
		}

	}
}