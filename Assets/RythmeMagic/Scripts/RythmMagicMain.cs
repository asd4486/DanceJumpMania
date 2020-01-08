using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
    public class RythmMagicMain : MonoBehaviour
    {
        UIMain uiMain;
        GameManager gameMgr;

        int score;
        int combo;

        public bool GameOver { get; private set; }

        [SerializeField] AudioSource mainAudio;
        [SerializeField] MusicSheetObject musicSheet;
        int nowBeat;

        float totalDuration;
        float playingTimer;

        [SerializeField] MarkerBase markerPrefab;
        [SerializeField] MarkerHold markerHoldPrefab;
        [SerializeField] MarkerTwoHand markerTwoHandPrefab;
        [SerializeField] MarkerHoldTwoHand markerHoldTwoHandPrefab;

        [SerializeField] Transform markerParent;

        //[SerializeField] int numBeatsPerSegment = 16;
        double nextEventTime;
        float bpm;

        // Start is called before the first frame update
        void Start()
        {
            uiMain = FindObjectOfType<UIMain>();
            gameMgr = FindObjectOfType<GameManager>();

            mainAudio.clip = musicSheet.music;
            totalDuration = mainAudio.clip.length + gameMgr.markerTime;

            GameOver = true;

            bpm = UniBpmAnalyzer.AnalyzeBpm(mainAudio.clip);
            nextEventTime = AudioSettings.dspTime + 60.0f / bpm;
        }

        Coroutine startGameCoroutine;
        internal void StartGame()
        {
            if (startGameCoroutine != null) return;

            startGameCoroutine = StartCoroutine(StartGameCoroutine());
        }

        IEnumerator StartGameCoroutine()
        {
            //init all values
            GameOver = false;
            score = 0;

            //for adjust speed
            var startTime = musicSheet.beatList[0].startTime;
            if (startTime < gameMgr.markerTime)
                yield return new WaitForSeconds(gameMgr.markerTime - startTime);

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
        }

        public void LogTime()
        {
            Debug.Log(playingTimer);
        }

        void SpawnNewMarkers(MusicSheetObject.Beat beat)
        {
            if (beat.infos.Count < 1) return;

            foreach (var item in beat.infos)
            {
                var marker = markerPrefab;

                if(item.markerType == MarkerType.TwoHand)
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
    }
}