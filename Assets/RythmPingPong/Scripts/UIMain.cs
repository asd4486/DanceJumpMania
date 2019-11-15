using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RythmePingPong
{
    public class UIMain : MonoBehaviour
    {
        [SerializeField] Text textScore;
        [SerializeField] Text textMiss;

        [SerializeField] GameObject hpObject;
        [SerializeField] Image hpValue;

        [SerializeField] GameObject result;
        [SerializeField] GameObject gameCompleteText;
        [SerializeField] GameObject gameOverText;
        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            result.SetActive(false);
            hpObject.SetActive(true);

            SetHpValue(1);
            SetScoreText(0);
            SetScoreMissText(0);
        }

        public void SetScoreText(int score)
        {
            textScore.text = $"Score: {score}";
        }

        public void SetScoreMissText(int score)
        {
            textMiss.text = $"Miss: {score}";
        }

        public void SetHpValue(float value)
        {
            hpValue.fillAmount = value;
        }

        public void Complete()
        {
            result.SetActive(true);
            hpObject.SetActive(false);
            gameOverText.SetActive(false);
            gameCompleteText.SetActive(true);
        }

        internal void GameOver()
        {
            result.SetActive(true);
            hpObject.SetActive(false);
            gameCompleteText.SetActive(false);
            gameOverText.SetActive(true);
        }
    }
}