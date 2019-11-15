using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythmePingPong
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] PongButton startButton;

        public void Reset()
        {
            startButton.Reset();
        }
    }
}