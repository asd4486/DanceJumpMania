using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
    public class RythmMagicMain : MonoBehaviour
    {
        int score;
        public bool GameOver { get; private set; }

        [SerializeField] AudioSource mainAudio;
        [SerializeField] MusicSheetObject musicSheet;
        int nowBeat;

        float totalDuration;
        float musicPlayDuration;

        [SerializeField] AIMarker markerPrefab;
        [SerializeField] Transform markerParent;

        //for adjust marker speed
        [SerializeField] float AdjustmentSpeed;
        [SerializeField] float markerSpeed = 1;

        [SerializeField] int numBeatsPerSegment = 16;
        double nextEventTime;
        float bpm;

        // Start is called before the first frame update
        void Start()
        {
            mainAudio.clip = musicSheet.music;
            totalDuration = mainAudio.clip.length + markerSpeed;

            GameOver = true;

            bpm = UniBpmAnalyzer.AnalyzeBpm(mainAudio.clip);
            nextEventTime = AudioSettings.dspTime + 60.0f / bpm;

            StartGame();
        }

        internal void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        IEnumerator StartGameCoroutine()
        {
            //init all values
            GameOver = false;
            score = 0;
            
            //for adjust speed
            yield return new WaitForSeconds(markerSpeed);
            mainAudio.Play();
        }

        private void Update()
        {
            if (GameOver) return;

            musicPlayDuration += Time.fixedDeltaTime;
            if (musicSheet.beatList[nowBeat].time <= musicPlayDuration + markerSpeed)
            {
                Debug.Log(musicPlayDuration);
                SpawnNewMarkers(musicSheet.beatList[nowBeat]);
                nowBeat += 1;
            }

            double time = AudioSettings.dspTime;
            if (time + 1.0f > nextEventTime)
            {
                // Place the next event 16 beats from here at a rate of 140 beats per minute
                nextEventTime += 60.0f / bpm * numBeatsPerSegment;
            }
        }

        void SpawnNewMarkers(MusicSheetObject.Beat beat)
        {
            if (beat.items == null || beat.items.Length < 1) return;

            foreach (var item in beat.items)
            {
                var o = Instantiate(markerPrefab.gameObject);
                o.transform.SetParent(markerParent, true);
                var marker = o.GetComponent<AIMarker>();
                marker.Init(item, markerSpeed + AdjustmentSpeed);
            }
        }
    }
}