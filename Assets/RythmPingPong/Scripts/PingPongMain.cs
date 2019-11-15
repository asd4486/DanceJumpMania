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

            var pongs = FindObjectsOfType<AIPingPong>();
            foreach (var p in pongs)
                Destroy(p.gameObject);

            spawner.ResetSpawnPong();
            uiMain.gameObject.SetActive(false);
            uiMain.Init();
            startMenu.gameObject.SetActive(true);
            startMenu.Reset();

            //reset function for last picked object
            foreach (var r in pickedRackets)
                r.OnReturnToMenu();
            pickedRackets.Clear();
        }

        public void AddMiss()
        {
            scoreMiss += 1;
            uiMain.SetScoreMissText(scoreMiss);
        }

        public void AddScore()
        {
            if (GameOver) return;
            score += 1;
            uiMain.SetScoreText(score);

            if (score != 0 && score % 7 == 0)
                spawner.LevelUp();

            if (score >= 168)
            {
                foreach (var r in pickedRackets)
                    r.FreezePositions(false);
                GameOver = true;
                uiMain.Complete();
            }
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
                GameOver = true;
                uiMain.GameOver();
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