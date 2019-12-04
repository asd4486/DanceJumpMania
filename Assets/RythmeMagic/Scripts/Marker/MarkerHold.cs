using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathCreation;

namespace RythhmMagic
{
	public class MarkerHold : MarkerBase
	{
		MarkerLine markerLine;

		//for moving Marker
		float movePathSpeed;

		float oneSecSpeed;
		float markerDuration;

		public override void Init(MusicSheetObject.BeatItem beat, float beatTime, float distance, float markDuration)
		{
			markerLine = GetComponentInChildren<MarkerLine>();

			//create holding road
			if (beat.holdingPos != null)
			{
				List<Vector3> posList = new List<Vector3>();
				posList.Add(Vector3.zero);

				oneSecSpeed = distance / markDuration;
				//get hold duration
				markerDuration = beat.holdingPos[beat.holdingPos.Length - 1].time - beatTime;

				foreach (var p in beat.holdingPos)
				{
					//get holding road lenght
					var roadLenght = oneSecSpeed * (p.time - beatTime);
					var adjustPos = p.pos - beat.startPos;
					posList.Add(new Vector3(adjustPos.x, adjustPos.y, roadLenght));
				}

				//draw road
				markerLine.GenerateMesh(posList.ToArray());
			}

			base.Init(beat, beatTime, distance, markDuration);
		}

		protected override void Update()
		{
			if (!startMove || isDestroied) return;

			if (transform.localPosition.z <= 0)
			{
				FollowPath();
			}
		}

		float pathTimer;
		void FollowPath()
		{
			if (pathTimer >= 1)
			{
				isDestroied = true;
				Destroy(gameObject, 0.1f);
				return;
			}

			markerLine.transform.position -= transform.forward * oneSecSpeed * Time.deltaTime;

			pathTimer += Time.deltaTime / markerDuration;
			var pathPos = markerLine.vectexPath.GetPointAtTime(pathTimer, EndOfPathInstruction.Stop);
			myCol.transform.position = new Vector3(pathPos.x, pathPos.y, transform.position.z);

			//transform.rotation = path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
		}
	}
}
