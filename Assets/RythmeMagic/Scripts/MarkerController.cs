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

        SpriteRenderer rendrerer;
        [SerializeField] Material defaultMat;
        [SerializeField] Material triggerMat;
        [SerializeField] Material twoHandsMat;
        [SerializeField] ParticleSystem fxTrigger;

        bool touchOtherHand;

        private void Awake()
        {
            main = FindObjectOfType<RythmMagicMain>();
            myCol = GetComponent<Collider>();
            rendrerer = GetComponentInChildren<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            myCol.enabled = !main.GameOver;

            transform.position = new Vector3(currentHand.transform.position.x, currentHand.transform.position.y, transform.position.z);

            if (currentHand.IsGrabbingWithType(GrabTypes.Pinch))
                controlMarkerType = MarkerType.Trigger;
            else
                controlMarkerType = MarkerType.Default;

            ChangeMat();
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
    }
}