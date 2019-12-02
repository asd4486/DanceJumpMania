using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RyhthmCircle
{
	public class RythmCircleMain : MonoBehaviour
	{
		int score;

		public bool GameOver { get; private set; }


		[SerializeField] int numBeatsPerSegment = 16;
		double nextEventTime;
		[SerializeField] AudioSource mainAudio;
		float bpm;

		[SerializeField] AIMarker markerPrefab;
		[SerializeField] Transform markerParent;
		//for adjust marker speed
		[SerializeField] float AdjustmentSpeed;

		// Start is called before the first frame update
		void Start()
		{
			GameOver = true;

			bpm = UniBpmAnalyzer.AnalyzeBpm(mainAudio.clip);
			nextEventTime = AudioSettings.dspTime + 60.0f / bpm;

			StartGame();
		}

		internal void StartGame()
		{
			mainAudio.Play();

			//init all values
			GameOver = false;
			score = 0;
		}

		private void Update()
		{
			if (GameOver) return;

			double time = AudioSettings.dspTime;
			if (time + 1.0f > nextEventTime)
			{
				// We are now approx. 1 second before the time at which the sound should play,
				// so we will schedule it now in order for the system to have enough time
				// to prepare the playback at the specified time. This may involve opening
				// buffering a streamed file and should therefore take any worst-case delay into account.
				//audioSources[flip].clip = clips[flip];
				//audioSources[flip].PlayScheduled(nextEventTime);

				//Debug.Log("Scheduled source " + flip + " to start at time " + nextEventTime);

				// Place the next event 16 beats from here at a rate of 140 beats per minute
				nextEventTime += 60.0f / bpm * numBeatsPerSegment;
				SpawnNewMarker();
			}
		}

		void SpawnNewMarker()
		{
			var o = Instantiate(markerPrefab.gameObject, Vector3.zero, Quaternion.identity);
			o.transform.SetParent(markerParent, false);
			var marker = o.GetComponent<AIMarker>();
			marker.Init(60.0f / bpm * numBeatsPerSegment + AdjustmentSpeed);
		}
	}
}