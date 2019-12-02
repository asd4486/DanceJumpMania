using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Valve.VR.InteractionSystem;
using System;

namespace RythmePingPong
{
    public class AIRacket : MonoBehaviour
    {
		RythmPingPongMain main;

        AudioSource myAudio;
        Throwable throwable;

        [SerializeField] Transform racketMenu;
        Vector3 menuPos;
        Vector3 menuRotation;
        Rigidbody rb;

        private void Start()
        {
            main = FindObjectOfType<RythmPingPongMain>();

            myAudio = GetComponent<AudioSource>();
            rb = GetComponent<Rigidbody>();
            throwable = GetComponent<Throwable>();
            //Physics.IgnoreLayerCollision(8, 10);

            menuPos = transform.position;
            menuRotation = transform.eulerAngles;
            OnReturnToMenu();

            throwable.onHeldUpdate.AddListener(OnAttaching);
        }

        private void Update()
        {
            //adjust position
            if (!throwable.Attached &&
                (Mathf.Abs(transform.position.x) > 3 || transform.position.y < -1 || Mathf.Abs(transform.position.z) > 2f))
            {
                //throw racket to return menu when game over
                if (main.GameOver && main.GamePlaying)
                {
                    main.ReturnToMenu();
                }
            }

            if (!main.GamePlaying)
            {
                transform.eulerAngles += new Vector3(0, 20 * Time.deltaTime, 0);
            }
        }

        public void OnReturnToMenu()
        {
            SetKinematic(true);
            transform.SetParent(racketMenu);
            transform.DOMove(menuPos, 0.4f);
            transform.DORotate(menuRotation, 0.4f);
            transform.eulerAngles = new Vector3(-40, 0, 0);

            throwable.onPickUp.RemoveAllListeners();
            throwable.onDetachFromHand.RemoveAllListeners();

            throwable.onPickUp.AddListener(OnAttached);
            throwable.onDetachFromHand.AddListener(OnDetachMoveToMenu);
        }

        public void OnGameStart()
        {
            FreezePositions(true);
            throwable.onPickUp.RemoveAllListeners();
            throwable.onDetachFromHand.RemoveAllListeners();

            throwable.onDetachFromHand.AddListener(() => SetKinematic(false));
        }

        public void FreezePositions(bool freeze)
        {
            rb.constraints = freeze ? RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ : RigidbodyConstraints.None;
        }

        void SetKinematic(bool kinematic)
        {
            rb.isKinematic = kinematic;
        }

        void OnAttached()
        {
            main.PickRacket(this);
        }

        void OnAttaching(Hand hand)
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
        }

        void OnDetachMoveToMenu()
        {
            main.DropRacket(this);

            SetKinematic(true);
            transform.SetParent(racketMenu);
            transform.DOMove(menuPos, 0.4f);
            transform.DORotate(menuRotation, 0.4f);
        }

        private void OnCollisionEnter(Collision collision)
        {
   //         if (collision.gameObject.GetComponent<AIPingPong>() != null && !myAudio.isPlaying)
			//{
			//	myAudio.Play();
			//}			
        }
    }
}