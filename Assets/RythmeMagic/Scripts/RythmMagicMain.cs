using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
    public class RythmMagicMain : MonoBehaviour
    {
        MenuUI playerUI;
        UIMain uiMain;
        GameManager gameMgr;

        public bool GameOver { get; private set; }

        int nowBeat;
        int score;
        int combo;

        [SerializeField] AudioSource mainAudio;
        [SerializeField] MusicSheetObject musicSheet;

        float totalDuration;
        float playingTimer;
        float completeTime;

        [SerializeField] MarkerBase markerPrefab;
        [SerializeField] MarkerHold markerHoldPrefab;
        [SerializeField] MarkerTwoHand markerTwoHandPrefab;
        [SerializeField] MarkerHoldTwoHand markerHoldTwoHandPrefab;

        [SerializeField] Transform markerParent;

        //[SerializeField] int numBeatsPerSegment = 16;
        double nextEventTime;
        float bpm;

        Coroutine completeCoroutine;

        // Start is called before the first frame update
        void Start()
        {
            playerUI = FindObjectOfType<MenuUI>();
            uiMain = FindObjectOfType<UIMain>();
            gameMgr = FindObjectOfType<GameManager>();

            mainAudio.clip = musicSheet.music;
            totalDuration = mainAudio.clip.length + gameMgr.markerTime;

            GameOver = true;

            bpm = UniBpmAnalyzer.AnalyzeBpm(mainAudio.clip);
            nextEventTime = AudioSettings.dspTime + 60.0f / bpm;
            completeTime = musicSheet.completeTime > 0 ? musicSheet.completeTime : musicSheet.music.length;
        }

        Coroutine startGameCoroutine;
        public void StartGame()
        {
            if (startGameCoroutine != null) return;

            startGameCoroutine = StartCoroutine(StartGameCoroutine());
        }

        IEnumerator StartGameCoroutine()
        {
            playerUI.ActiveUI(false);

            playingTimer =  nowBeat = score = combo = 0;
            //init all values
            GameOver = false;

            //for adjust speed
            var startTime = musicSheet.beatList[0].startTime;
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
            if (nowBeat < musicSheet.beatList.Count && musicSheet.beatList[nowBeat].startTime - gameMgr.markerTime <= playingTimer)
            {
                SpawnNewMarkers(musicSheet.beatList[nowBeat]);
                nowBeat += 1;
            }

            double time = AudioSettings.dspTime;
            if (time + 1.0f > nextEventTime)
            {
                // Place the next event 16 beats from here at a rate of 140 beats per minute
                nextEventTime += 60.0f / bpm /** numBeatsPerSegment*/;
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
                o.transform.SetParent(markerParent, true);
                o.GetComponent<MarkerBase>().Init(item, beat.startTime);
            }
        }

        public void AddScore()
        {
            score += 10;
            combo += 1;
            uiMain.SetScore(score);
            uiMain.SetCombo(combo);
        }

        public void BreakCombo()
        {
            combo = 0;
            uiMain.BreakCombo();
        }

        IEnumerator CompleteCorou()
        {
            mainAudio.DOFade(0, 4f);
            yield return new WaitForSeconds(4f);

            mainAudio.Stop();
            playerUI.ActiveUI(true);
            GameOver = true;

            completeCoroutine = null;
        }
    }
}