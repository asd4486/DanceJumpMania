using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
	public class MarkerBase : MonoBehaviour
	{
		MusicSheetObject.BeatItem currentBeat;
		protected Collider myCol;

		protected bool startMove;
		protected bool isDestroied;
		// Start is called before the first frame update
		void Awake()
		{
			myCol = GetComponentInChildren<Collider>();
			myCol.enabled = false;
		}

		public virtual void Init(MusicSheetObject.BeatItem beat, float beatTime, float distance, float markDuration)
		{
			transform.localPosition = new Vector3(beat.startPos.x, beat.startPos.y, distance);
			StartCoroutine(ActiveColCoroutine(markDuration - 0.1f));

			startMove = true;
			DOTween.To(() => transform.localPosition, x => transform.localPosition = x, new Vector3(transform.localPosition.x, transform.localPosition.y, 0),
				markDuration).SetEase(Ease.Linear);
		}

		IEnumerator ActiveColCoroutine(float time)
		{
			yield return new WaitForSeconds(time);
			myCol.enabled = true;
		}

		protected virtual void Update()
		{
			if (!startMove || isDestroied) return;

			if (transform.localPosition.z <= 0)
			{
				isDestroied = true;
				GetComponent<AudioSource>().Play();
				Destroy(gameObject, 0.3f);
			}
		}

		protected virtual void OnTouchMarker()
		{
			isDestroied = true;
			GetComponentInChildren<MeshRenderer>().material.color = Color.black;
			GetComponent<AudioSource>().Play();
			Destroy(gameObject, 0.1f);
		}


		private void OnCollisionStay(Collision collision)
		{
			Debug.Log("touch");
			if (collision.gameObject.GetComponent<MarkerController>() != null)
			{
				OnTouchMarker();
			}
		}
	}
}