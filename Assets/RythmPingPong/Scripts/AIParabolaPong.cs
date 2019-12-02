using RythmePingPong.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythmePingPong
{
	public class AIParabolaPong : MonoBehaviour
	{
		bool touchedRacket;
		ParabolaController parabola;
		AudioSource myAudio;

		[SerializeField] Transform point1;
		[SerializeField] Transform point2;
		[SerializeField] Transform point3;

		[SerializeField] Transform point4;
		[SerializeField] Transform point5;
		[SerializeField] Transform point6;

		float spawnMaxAngle;

		[SerializeField] float adjacentMaxPoint;
		[SerializeField] float opposite;

 		[SerializeField] AudioClip touchRacketSound;

		// Start is called before the first frame update
		void Awake()
		{
			parabola = GetComponentInChildren<ParabolaController>();
			myAudio = GetComponent<AudioSource>();

			float adjacent = -adjacentMaxPoint;

			point1.transform.localPosition = new Vector3(0, 1.5f, opposite);

			spawnMaxAngle = Mathf.Abs(Mathf.Atan2(opposite, adjacent) * (180 / Mathf.PI));
			if (spawnMaxAngle > 90) spawnMaxAngle -= 90;

			var eachTriZPos = opposite / 2;
			//set random angle
			var randAngle = Random.Range(-spawnMaxAngle, spawnMaxAngle);
			var angleC = (90 - Mathf.Abs(randAngle)) * (Mathf.PI / 180);

			var xPos = eachTriZPos / Mathf.Tan(angleC) * (randAngle < 0 ? -1 : 1);
			point2.transform.localPosition = new Vector3(xPos / 2, 1.5f, eachTriZPos + eachTriZPos / 2);
			point3.transform.localPosition = point4.transform.localPosition = new Vector3(xPos, 0.8f, eachTriZPos);

			var xPos2 = eachTriZPos / Mathf.Tan(angleC) * (randAngle < 0 ? -1 : 1);
			point5.transform.localPosition = new Vector3(xPos + xPos2 / 2, 1.4f, eachTriZPos / 2);
			point6.transform.localPosition = new Vector3(xPos + xPos2, 1.2f, 0);
		}

		public void Init(float duration)
		{
			parabola.totalDuration = duration;
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
			Destroy(gameObject, 0.3f);
		}

		public void OnTouchedRacket()
		{
			if (touchedRacket) return;

			touchedRacket = true;
			myAudio.clip = touchRacketSound;
			myAudio.Play();
		}
	}
}