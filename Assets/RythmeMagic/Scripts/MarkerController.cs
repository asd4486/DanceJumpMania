using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


namespace RythhmMagic
{
    public class MarkerController : MonoBehaviour
    {
        RythmMagicMain main;

        Collider myCol;
        [SerializeField] Hand currentHand;
        public Hand CurrentHand { get { return currentHand; } }

        public MarkerType controlMarkerType { get; private set; }
        public Material currentMat { get; set; }
        public bool TriggerDown { get; private set; }

        SpriteRenderer rendrerer;
        [SerializeField] Material defaultMat;
        [SerializeField] Material triggerMat;
        [SerializeField] Material twoHandsMat;
        [SerializeField] ParticleSystem fxTrigger;

        bool touchOtherHand;

        float rotateSpeed;
        [SerializeField] float maxRotateSpeed;

        private void Awake()
        {
            main = FindObjectOfType<RythmMagicMain>();
            myCol = GetComponent<Collider>();
            rendrerer = GetComponentInChildren<SpriteRenderer>();

            currentMat = defaultMat;
        }

        // Update is called once per frame
        void Update()
        {
            ScaleStars();

            transform.position = new Vector3(currentHand.transform.position.x, currentHand.transform.position.y, transform.position.z);

            if (currentHand.IsGrabbingWithType(GrabTypes.Pinch))
            {
                TriggerDown = true;
                controlMarkerType = MarkerType.Trigger;
            }

            else
            {
                TriggerDown = false;
                controlMarkerType = MarkerType.Default;
            }

            RotateController();
            ChangeMat();
        }

        bool gameOver;
        void ScaleStars()
        {
            if (gameOver == main.GameOver)
                return;

            gameOver = main.GameOver;
            transform.DOScale(gameOver ? new Vector3(0.5f, 0.5f, 0.5f) : Vector3.one, 0.1f).SetEase(Ease.OutElastic);
        }

        void RotateController()
        {
            if (rotateSpeed > maxRotateSpeed)
                rotateSpeed = maxRotateSpeed;
            else
            {
                if (rotateSpeed > 0)
                    rotateSpeed -= 15;
                else
                {
                    rotateSpeed = 0;
                    return;
                }
            }

            if (currentHand.handType == Valve.VR.SteamVR_Input_Sources.LeftHand)
                rendrerer.transform.eulerAngles += new Vector3(0, 0, rotateSpeed) * Time.deltaTime;
            else
                rendrerer.transform.eulerAngles -= new Vector3(0, 0, rotateSpeed) * Time.deltaTime;
        }

        public void TouchMarker()
        {
            rotateSpeed += 500;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<MarkerController>() != null)
                touchOtherHand = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<MarkerController>() != null)
                touchOtherHand = false;
        }

        void ChangeMat()
        {
            if (touchOtherHand)
            {
                rendrerer.material = currentMat = twoHandsMat;
                if (fxTrigger.isPlaying) fxTrigger.Stop();
                return;
            }

            switch (controlMarkerType)
            {
                case MarkerType.Default:
                    rendrerer.material = currentMat = defaultMat;
                    if (fxTrigger.isPlaying) fxTrigger.Stop();
                    break;
                case MarkerType.Trigger:
                    rendrerer.material = currentMat = triggerMat;
                    if (!fxTrigger.isPlaying) fxTrigger.Play();
                    break;
            }
        }

        public void Vibrate()
        {
            currentHand.TriggerHapticPulse(0.1f, 60f, 0.5f);
        }
    }
}