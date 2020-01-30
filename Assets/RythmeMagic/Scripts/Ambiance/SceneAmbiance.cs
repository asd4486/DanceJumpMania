using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
    public class SceneAmbiance : MonoBehaviour
    {
        GameManager gameMgr;
        [SerializeField] Transform gameZone;

        bool isPlaying;
        [SerializeField] AIDoor doorPrefab;

        [SerializeField] ParticleSystem fxAmbiance;
        [SerializeField] ParticleSystem fxAmbianceInGame;

        [SerializeField] int numBeatsPerSegment = 4;
        double nextEventTime;
        float BPM;

        private void Awake()
        {
            gameMgr = FindObjectOfType<GameManager>();
        }

        public void SetBpm(float bpm)
        {
            BPM = bpm;
            nextEventTime = AudioSettings.dspTime + 60.0f / BPM * numBeatsPerSegment;
        }

        public void PlayAmbianceFx(bool inGame)
        {
            isPlaying = inGame;
            if (!inGame)
            {
                fxAmbiance.Play();
                fxAmbianceInGame.Stop();
            }
            else
            {
                fxAmbianceInGame.Play();
                fxAmbiance.Stop();
            }
        }

        private void Update()
        {
            if (!isPlaying)
                return;

            double time = AudioSettings.dspTime;
            if (time > nextEventTime - gameMgr.markerTime)
            {
                // Place the next event 16 beats from here at a rate of 140 beats per minute
                nextEventTime += 60.0f / BPM * numBeatsPerSegment;
                CreateDoor();
            }
        }

        void CreateDoor()
        {
            var door = Instantiate<AIDoor>(doorPrefab);
            door.transform.SetParent(gameZone, false);
            door.Init(gameMgr.markerDistance * 2, gameMgr.MarkerSpeed * 2, gameMgr.markerTime);
        }
    }
}