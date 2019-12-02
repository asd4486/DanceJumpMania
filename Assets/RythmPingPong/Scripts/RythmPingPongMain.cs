using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace RythmePingPong
{
	public class RythmPingPongMain : MonoBehaviour
	{
		[SerializeField] StartMenu startMenu;
		//Transform spawner;
		UIMain uiMain;

		int score;

		public bool GameOver { get; private set; }
		public bool GamePlaying { get { return startMenu.gameObject.activeSelf == false; } }

		List<AIRacket> pickedRackets = new List<AIRacket>();

		[SerializeField] int numBeatsPerSegment = 16;
		private double nextEventTime;
		[SerializeField] AudioSource mainAudio;
		float bpm;

		[SerializeField] float adjustSpeed = 0.1f;
		[SerializeField] AIParabolaPong pongPrefab;
		[SerializeField] Transform pongParent;

        [SerializeField] Transform beatLine1;

		// Start is called before the first frame update
		void Start()
		{
			GameOver = true;
			uiMain = FindObjectOfType<UIMain>();

			uiMain.gameObject.SetActive(false);
			startMenu.gameObject.SetActive(true);

			bpm = UniBpmAnalyzer.AnalyzeBpm(mainAudio.clip);
			nextEventTime = AudioSettings.dspTime + 60.0f / bpm;
		}

		internal void StartGame()
		{
			foreach (var r in pickedRackets)
				r.OnGameStart();

			mainAudio.Play();

			//init all values
			GameOver = false;

			score = 0;

			uiMain.gameObject.SetActive(true);
			startMenu.gameObject.SetActive(false);
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
				SpawnPong();
			}
		}

		void SpawnPong()
		{
			var o = Instantiate(pongPrefab.gameObject, Vector3.zero, Quaternion.identity);
			o.transform.SetParent(pongParent, false);
			var parabolaPong = o.GetComponent<AIParabolaPong>();
            parabolaPong.Init(60.0f / bpm * numBeatsPerSegment + adjustSpeed, beatLine1.transform.position.y);
		}

		internal void ReturnToMenu()
		{
			//return if player drag a racket
			if (pickedRackets.Any(r => r.GetComponent<Throwable>().Attached) || startMenu.gameObject.activeSelf)
				return;

			startMenu.gameObject.SetActive(true);
			startMenu.Reset();
			//spawner.ResetSpawnPong();
			uiMain.gameObject.SetActive(false);
			uiMain.Init();

			var pongs = FindObjectsOfType<AIPingPong>();
			foreach (var p in pongs)
				Destroy(p.gameObject);

			//reset function for last picked object
			foreach (var r in pickedRackets)
				r.OnReturnToMenu();

			pickedRackets.Clear();
		}

		public void AddScore()
		{
			if (GameOver) return;
			score += 2;
			uiMain.SetScoreText(score);

			//if (score != 0 && score % 20 == 0)
			//	spawner.LevelUp();
		}

		void BonusTime()
		{
		}

		void BonsuTimeFinish()
		{
		}

		public void PickRacket(AIRacket racket)
		{
			if (!pickedRackets.Contains(racket)) pickedRackets.Add(racket);
		}

		public void DropRacket(AIRacket racket)
		{
			if (pickedRackets.Contains(racket)) pickedRackets.Remove(racket);
		}
	}
}