using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace RythmePingPong
{
    public class PongButton : MonoBehaviour
    {
        Rigidbody rb;
        AudioSource myAudio;
        PingPongMain main;
        // Start is called before the first frame update
        void Start()
        {
            myAudio = GetComponent<AudioSource>();
            rb = GetComponent<Rigidbody>();

            main = FindObjectOfType<PingPongMain>();
        }

        public void Reset()
        {
            transform.position = new Vector3(0, 1.1f, 1.3f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            rb.AddForce(new Vector3(0, 1.3f, 0));
            if (rb.velocity.magnitude > 0.5f && !myAudio.isPlaying)
            {
                myAudio.Play();
            }

            if (collision.gameObject.GetComponent<AIPingPongRacket>() != null && collision.gameObject.GetComponent<Throwable>().Attached)
                main.StartGame();
        }
    }
}