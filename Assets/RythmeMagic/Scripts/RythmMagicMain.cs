using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
	public enum HitJuge
	{
		Prefect,
		Good,
		Bad,
		Miss
	}

	public class RythmMagicMain : MonoBehaviour
	{
		MainMenu menuUI;
		UIMain uiMain;
		GameManager gameMgr;
		[SerializeField] SceneAmbiance ambiance;

		public bool GameOver { get; private set; }

		int nowBeat;

		public ScoreData ScoreDt { get; private set; }

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
			ScoreDt = new ScoreData();

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
			ScoreDt.Reset();
			playingTimer = nowBeat = 0;

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

		public void HitMarker(HitJuge jugement)
		{
			bool breakCombo = false;
			switch (jugement)
			{
				case HitJuge.Prefect:
					ScoreDt.score += 10;
					ScoreDt.prefectCount += 1;
					break;
				case HitJuge.Good:
					ScoreDt.score += 8;
					ScoreDt.goodCount += 1;
					break;
				case HitJuge.Bad:
					breakCombo = true;
					ScoreDt.badCount += 1;
					break;
				case HitJuge.Miss:
					breakCombo = true;
					ScoreDt.missCount += 1;
					break;
			}

			if (breakCombo)
			{
				//play break combo animation
				if (ScoreDt.combo > 0)
				{
					ScoreDt.combo = 0;
					uiMain.BreakCombo();
				}
				return;
			}

			//add combo
			ScoreDt.combo += 1;
			if (ScoreDt.combo > ScoreDt.maxCombo)
				ScoreDt.maxCombo = ScoreDt.combo;

			uiMain.SetScore(ScoreDt.score);
			uiMain.SetCombo(ScoreDt.combo);
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
			menuUI.ShowResult();

			GameOver = true;
			completeCoroutine = null;
		}
	}
}