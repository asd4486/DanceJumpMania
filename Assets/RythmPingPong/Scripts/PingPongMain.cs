using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace RythmePingPong
{
    public class PingPongMain : MonoBehaviour
    {
        [SerializeField] StartMenu startMenu;
        PongSpawner spawner;
        UIMain uiMain;

        int score;
        int scoreMiss;

        float nowHp = 100;
        float totalHp = 100;

        public bool GameOver { get; private set; }
        public bool GamePlaying { get { return startMenu.gameObject.activeSelf == false; } }

        List<AIPingPongRacket> pickedRackets = new List<AIPingPongRacket>();

        // Start is called before the first frame update
        void Start()
        {
            GameOver = true;
            uiMain = FindObjectOfType<UIMain>();
            spawner = FindObjectOfType<PongSpawner>();

            uiMain.gameObject.SetActive(false);
            startMenu.gameObject.SetActive(true);
        }

		internal void StartGame()
        {
            foreach (var r in pickedRackets)
                r.OnGameStart();

            //init all values
            GameOver = false;
            nowHp = totalHp;
            score = scoreMiss = 0;

            spawner.StartSpawnPong();
            uiMain.gameObject.SetActive(true);
            startMenu.gameObject.SetActive(false);
        }

        internal void ReturnToMenu()
        {
            //return if player drag a racket
            if (pickedRackets.Any(r => r.GetComponent<Throwable>().Attached) || startMenu.gameObject.activeSelf)
                return;

            startMenu.gameObject.SetActive(true);
            startMenu.Reset();
            spawner.ResetSpawnPong();
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
            score += 1;
            uiMain.SetScoreText(score);

            if (score != 0 && score % 10 == 0)
                spawner.LevelUp();
        }

        public void AddMiss()
        {
            //scoreMiss += 1;
            //uiMain.SetScoreMissText(scoreMiss);

            //if (scoreMiss >= 100)
            //{
            //    foreach (var r in pickedRackets)
            //        r.FreezePositions(false);
                
            //    spawner.SetSpawnDelay(0);
            //    uiMain.GameOver();
            //    GameOver = true;
            //}
        }

        public void Hurt()
        {
            if (GameOver) return;
            nowHp -= 15;
            var value = nowHp <= 0 ? 0 : nowHp / totalHp;
            uiMain.SetHpValue(value);

            //game over
            if (nowHp <= 0)
            {
                foreach (var r in pickedRackets)
                    r.FreezePositions(false);
                
                spawner.SetSpawnDelay(0);
                uiMain.GameOver();
                GameOver = true;
            }
        }

        public void PickRacket(AIPingPongRacket racket)
        {
            if (!pickedRackets.Contains(racket)) pickedRackets.Add(racket);
        }

        public void DropRacket(AIPingPongRacket racket)
        {
            if (pickedRackets.Contains(racket)) pickedRackets.Remove(racket);
        }
    }
}