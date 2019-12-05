using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
	public class MarkerBase : MonoBehaviour
	{
		MusicSheetObject.BeatInfo currentBeat;
		protected Collider myCol;
		[SerializeField] protected ParticleSystem fxTouch;

		protected bool startMove;
		// Start is called before the first frame update
		void Awake()
		{
			myCol = GetComponentInChildren<Collider>();
			myCol.enabled = false;
		}

		public virtual void Init(MusicSheetObject.BeatInfo beat, float beatTime)
		{
			transform.localPosition = new Vector3(beat.posList[0].pos.x, beat.posList[0].pos.y, GameManager.Instance.markerDistance);
			StartCoroutine(ActiveColCoroutine(GameManager.Instance.MarkerSpeed - 0.1f));

			startMove = true;
			DOTween.To(() => transform.localPosition, x => transform.localPosition = x, new Vector3(transform.localPosition.x, transform.localPosition.y, 0),
				GameManager.Instance.MarkerSpeed).SetEase(Ease.Linear);
		}

		IEnumerator ActiveColCoroutine(float time)
		{
			yield return new WaitForSeconds(time);
			myCol.enabled = true;
		}

		protected virtual void Update()
		{
			if (!startMove || !myCol.enabled) return;

			if (transform.localPosition.z <= 0)
			{
				GetComponent<AudioSource>().Play();
				Destroy(gameObject, 0.3f);
			}
		}

		protected virtual void OnHitMarker()
		{
			myCol.enabled = false;
			myCol.transform.DOScale(Vector3.zero, 0.1f);
			fxTouch.Play();
			Destroy(gameObject, 0.2f);
		}

		private void OnCollisionStay(Collision collision)
		{
			if (collision.gameObject.GetComponent<MarkerController>() != null)
			{
				OnHitMarker();
			}
		}
	}
}