using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace RythmePingPong
{
    public class PongButton : MonoBehaviour
    {
        PingPongMain main;
        // Start is called before the first frame update
        void Start()
        {
            main = FindObjectOfType<PingPongMain>();
        }

        public void Reset()
        {
            transform.position = new Vector3(0, 1.1f, 1.3f);          
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<AIPingPongRacket>() != null && collision.gameObject.GetComponent<Throwable>().Attached)
                main.StartGame();
        }
    }
}