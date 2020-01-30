using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
	public class RythmMagicMain : MonoBehaviour
	{
		MainMenu menuUI;
		UIMain uiMain;
		GameManager gameMgr;
		[SerializeField] SceneAmbiance ambiance;

		public bool GameOver { get; private set; }

		int nowBeat;

		int score;
		int goodCount;
		int missCount;

		int combo;
		int maxCombo;

		[SerializeField] AudioSource mainAudio;
		MusicSheetObject currentSheet;
        public float BPM { get; private set; }

        float totalDuration;
		float playingTimer;
		float completeTime;

		[SerializeField] MarkerBase markerPrefab;
		[SerializeField] MarkerHold markerHoldPrefab;
		[SerializeField] MarkerTwoHand markerTwoHandPrefab;
		[SerializeField] MarkerHoldTwoHand markerHoldTwoHandPrefab;

		[SerializeField] GameObject fxSpawn;
		[SerializeField] GameObject fxSpawnTrigger;
		[SerializeField] GameObject fxSpawnTwoHand;

		[SerializeField] Transform gameZone;

		Coroutine completeCoroutine;

		// Start is called before the first frame update
		void Start()
		{
			menuUI = FindObjectOfType<MainMenu>();
			uiMain = FindObjectOfType<UIMain>();
			uiMain.gameObject.SetActive(false);

			gameMgr = FindObjectOfType<GameManager>();

			GameOver = true;
		}

		Coroutine startGameCoroutine;
		public void StartGame(MusicSheetObject sheet)
		{
			if (startGameCoroutine != null) return;

			startGameCoroutine = StartCoroutine(StartGameCoroutine(sheet));
		}

		IEnumerator StartGameCoroutine(MusicSheetObject sheet)
		{
			uiMain.gameObject.SetActive(true);
			uiMain.Init();

			//setup music infos
			currentSheet = sheet;
			mainAudio.clip = currentSheet.music;
			totalDuration = mainAudio.clip.length + gameMgr.markerTime;

			completeTime = currentSheet.duration > 0 ? currentSheet.duration : currentSheet.music.length;
            BPM = UniBpmAnalyzer.AnalyzeBpm(currentSheet.music);

            yield return new WaitForSeconds(2f);

			menuUI.ActiveUI(false);

            //change amibance
            ambiance.SetBpm(BPM);
            ambiance.PlayAmbianceFx(true);

			//init all values
			goodCount = missCount = 0;
			playingTimer = nowBeat = score = combo = maxCombo = 0;

            GameOver = false;

			//for adjust speed
			var startTime = currentSheet.beatList[0].startTime;
			if (startTime < gameMgr.markerTime)
				yield return new WaitForSeconds(gameMgr.markerTime - startTime);

			mainAudio.volume = 1;
			mainAudio.Play();
			startGameCoroutine = null;
		}

		private void Update()
		{
			if (GameOver) return;

			playingTimer += Time.deltaTime;
			if (nowBeat < currentSheet.beatList.Count && currentSheet.beatList[nowBeat].startTime - gameMgr.markerTime <= playingTimer)
			{
				SpawnNewMarkers(currentSheet.beatList[nowBeat]);
				nowBeat += 1;
			}

			if (playingTimer >= completeTime + gameMgr.markerTime)
			{
				if (completeCoroutine == null)
					completeCoroutine = StartCoroutine(CompleteCorou());
			}
		}

		void SpawnNewMarkers(MusicSheetObject.Beat beat)
		{
			if (beat.infos.Count < 1) return;

			foreach (var item in beat.infos)
			{
				var marker = markerPrefab;
				var fx = item.markerType == MarkerType.Default ? fxSpawn : fxSpawnTrigger;

				if (item.markerType == MarkerType.TwoHand)
				{
					switch (item.beatType)
					{
						case BeatTypes.Default:
							marker = markerTwoHandPrefab;
							break;
						case BeatTypes.Holding:
							marker = markerHoldTwoHandPrefab;
							break;
					}
					fx = fxSpawnTwoHand;
				}
				else
				{
					switch (item.beatType)
					{
						case BeatTypes.Holding:
							marker = markerHoldPrefab;
							break;
					}
				}

				var o = Instantiate(marker.gameObject);
				o.transform.SetParent(gameZone, true);
				o.GetComponent<MarkerBase>().Init(item, beat.startTime);

				var newFx = Instantiate(fx);
				newFx.transform.SetParent(gameZone, true);
				newFx.transform.localPosition = new Vector3(item.posList[0].pos.x, item.posList[0].pos.y, gameMgr.markerDistance);
				Destroy(newFx, 0.25f);
			}
		}

		public void AddScore()
		{
			score += 10;
			goodCount += 1;

			combo += 1;
			if (combo > maxCombo)
				maxCombo = combo;

			uiMain.SetScore(score);
			uiMain.SetCombo(combo);
		}

		public void BreakCombo()
		{
			//play break combo animation
			if (combo > 0)
			{
				combo = 0;
				uiMain.BreakCombo();
			}

			missCount += 1;
		}

		IEnumerator CompleteCorou()
		{
			mainAudio.DOFade(0, 4f);
			//change amibance
			ambiance.PlayAmbianceFx(false);

			yield return new WaitForSeconds(4f);

			mainAudio.Stop();

			uiMain.gameObject.SetActive(false);
			menuUI.ActiveUI(true);
			menuUI.ShowResult(goodCount, missCount, maxCombo, score);

			GameOver = true;
			completeCoroutine = null;
		}
	}
}