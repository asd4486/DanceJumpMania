using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RythhmMagic
{
	public class MarkerBase : MonoBehaviour
	{
		protected RythmMagicMain main;
		protected MusicSheetObject.BeatInfo currentBeat;
		protected Collider myCol;
		[SerializeField] protected ParticleSystem fxTouch;
		[SerializeField] protected ParticleSystem fxTouchTrigger;
		protected GameManager gameMgr;
		protected float markerSpeed;

		protected bool startMove;

		[SerializeField] protected SpriteRenderer markerRenderer;
		[SerializeField] protected Material defaultMat;
		[SerializeField] protected Material triggerMat;

		protected int rotateDirection;

		//timer for hitjuge
		//0 - 0.1 prefect
		//0.1 - ~ good
		protected float waitForHitTimer;

		void Awake()
		{
			main = FindObjectOfType<RythmMagicMain>();
			gameMgr = FindObjectOfType<GameManager>();
			markerSpeed = gameMgr.MarkerSpeed;

			myCol = GetComponentInChildren<Collider>();
			myCol.enabled = false;
		}

		public virtual void Init(MusicSheetObject.BeatInfo beat, float beatTime)
		{
			if (beat.markerType == MarkerType.Default) markerRenderer.material = defaultMat;
			else if (beat.markerType == MarkerType.Trigger) markerRenderer.material = triggerMat;

			currentBeat = beat;
			transform.localPosition = new Vector3(beat.posList[0].pos.x, beat.posList[0].pos.y, gameMgr.markerDistance);
			StartCoroutine(ActiveColCoroutine(gameMgr.markerTime));

			//set random rotate direction
			rotateDirection = Random.Range(-1, 2);
			if (rotateDirection == 0) rotateDirection = 1;

			var scale = markerRenderer.transform.localScale;
			markerRenderer.transform.localScale = Vector3.zero;
			markerRenderer.transform.DOScale(scale, 0.1f);

			startMove = true;
		}

		protected virtual IEnumerator ActiveColCoroutine(float time)
		{
			yield return new WaitForSeconds(time);
			myCol.enabled = true;
		}

		protected virtual void Update()
		{
			if (!startMove)
				return;

			markerRenderer.transform.Rotate(0, 0, main.BPM * rotateDirection * Time.deltaTime);
			transform.localPosition -= Vector3.forward * gameMgr.MarkerSpeed * Time.deltaTime;
			if (transform.localPosition.z <= 0)
			{
				startMove = false;
				StartCoroutine(WaitPlayerHitCoroutine());
			}
		}

		IEnumerator WaitPlayerHitCoroutine()
		{
			while (waitForHitTimer < 0.15f)
			{
				waitForHitTimer += Time.deltaTime;
				yield return null;
			}

			if (myCol.enabled)
			{
				myCol.enabled = false;
				markerRenderer.transform.DOScale(Vector3.zero, 0.1f);
				Destroy(gameObject, 0.1f);
				main.HitMarker(HitJuge.Miss, transform.position);
			}
		}

		protected virtual void OnHitMarker()
		{
			startMove = myCol.enabled = false;
			markerRenderer.transform.DOScale(Vector3.zero, 0.1f);

			if (currentBeat.markerType == MarkerType.TwoHand || hitedController.controlMarkerType == currentBeat.markerType)
			{
				if (currentBeat.markerType == MarkerType.Trigger)
					fxTouchTrigger.Play();
				else
					fxTouch.Play();

				main.HitMarker(waitForHitTimer <= 0.1f ? HitJuge.Prefect : HitJuge.Good, transform.position);
			}
			else
				main.HitMarker(HitJuge.Bad , transform.position);

			Destroy(gameObject, 0.2f);
		}

		protected MarkerController hitedController;
		protected virtual void OnTriggerStay(Collider col)
		{
			if (col.gameObject.GetComponent<MarkerController>() != null)
			{
				if (hitedController == null)
				{
					hitedController = col.gameObject.GetComponent<MarkerController>();
					OnHitMarker();

					hitedController.Vibrate();
				}
				else
					OnHitMarker();

				hitedController.TouchMarker();
			}
		}

		protected virtual void OnTriggerExit(Collider col)
		{
			if (!myCol.enabled) return;

			if (col.gameObject.GetComponent<MarkerController>() != null && col.gameObject.GetComponent<MarkerController>() == hitedController)
			{
				hitedController = null;
			}
		}
	}
}