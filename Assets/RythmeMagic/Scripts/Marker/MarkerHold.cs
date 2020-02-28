using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using DG.Tweening;

namespace RythhmMagic
{
	public class MarkerHold : MarkerBase
	{
		protected MarkerLine markerLine;
		protected float markerDuration;

		protected float countComboTimer;
		Coroutine breakComboCoroutine;

		bool? isComboBreaking;
		//for avoid hit controller null bug
		MarkerController lastHitController;

		private void Start()
		{
			countComboTimer = gameMgr.addScoreDelay;
		}

		public override void Init(MusicSheetObject.BeatInfo beat, float beatTime)
		{
			markerLine = GetComponentInChildren<MarkerLine>();

			if (beat.markerType == MarkerType.Default) markerLine.SetMaterial(defaultMat);
			else if (beat.markerType == MarkerType.Trigger) markerLine.SetMaterial(triggerMat);

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

		protected override IEnumerator ActiveColCoroutine(float time)
		{
			yield return new WaitForSeconds(time);
			myCol.enabled = true;
			BreakCombo();
		}

		protected void BreakCombo()
		{
			if (breakComboCoroutine != null)
				return;
			breakComboCoroutine = StartCoroutine(BreakComboCorou());
		}

		IEnumerator BreakComboCorou()
		{
			yield return new WaitForSeconds(0.1f);
			isComboBreaking = true;
			breakComboCoroutine = null;
		}

		protected void StopBreakCombo()
		{
			isComboBreaking = false;

			if (breakComboCoroutine == null)
				return;
			StopCoroutine(breakComboCoroutine);
			breakComboCoroutine = null;
		}

		protected override void Update()
		{
			if (!startMove) return;

			markerRenderer.transform.Rotate(0, 0, main.BPM * rotateDirection * Time.deltaTime);
			if (transform.localPosition.z > 0) transform.position -= transform.forward * markerSpeed * Time.deltaTime;
			else FollowRoadPath();

			if (isComboBreaking != null)
			{
				//calculate combo
				countComboTimer += Time.deltaTime;
				if (countComboTimer >= gameMgr.addScoreDelay)
				{
					if (isComboBreaking.Value) main.HitMarker(HitJuge.Miss, markerRenderer.transform.position);
					else
					{
						if (currentBeat.markerType == MarkerType.TwoHand)
							main.HitMarker(HitJuge.Prefect, markerRenderer.transform.position);
						else
						{
							if (lastHitController != null && lastHitController.controlMarkerType == currentBeat.markerType)
								main.HitMarker(HitJuge.Prefect, markerRenderer.transform.position);
							else
								main.HitMarker(HitJuge.Bad, markerRenderer.transform.position);
						}
					}

					countComboTimer = 0;
				}
			}
		}

		//timer for follow path (0 to 1)
		float pathTimer;
		void FollowRoadPath()
		{
			//finish
			if (pathTimer >= 1)
			{
				Destroy();
				return;
			}

			markerLine.transform.position -= transform.forward * markerSpeed * Time.deltaTime;

			pathTimer += Time.deltaTime / markerDuration;
			var pathPos = markerLine.vectexPath.GetPointAtTime(pathTimer, EndOfPathInstruction.Stop);
			markerRenderer.transform.position = new Vector3(pathPos.x, pathPos.y, transform.position.z);
		}

		void Destroy()
		{
			startMove = myCol.enabled = false;
			if (fxTouch.isPlaying) fxTouch.Stop();
			//avoid combo drop bug
			StopBreakCombo();

			markerRenderer.transform.DOScale(Vector3.zero, 0.1f);
			Destroy(gameObject, 0.2f);
		}

		protected override void OnHitMarker()
		{
			//play fx if is good hit
			if (currentBeat.markerType == MarkerType.TwoHand)
			{
				if (!fxTouch.isPlaying)
					fxTouch.Play();
			}
			else if (hitedController.controlMarkerType == currentBeat.markerType)
			{
				//save current controller
				if (hitedController.controlMarkerType == currentBeat.markerType) lastHitController = hitedController;

				if (currentBeat.markerType == MarkerType.Trigger)
				{
					if (!fxTouchTrigger.isPlaying)
						fxTouchTrigger.Play();
				}
				else
				{
					if (!fxTouch.isPlaying)
						fxTouch.Play();
				}
			}

			StopBreakCombo();
		}

		protected override void OnTriggerExit(Collider col)
		{
			if (!myCol.enabled) return;

			if (col.gameObject.GetComponent<MarkerController>() != null &&
				col.gameObject.GetComponent<MarkerController>() == hitedController)
			{
				hitedController = null;
				fxTouch.Stop();
				BreakCombo();
			}
		}
	}
}
