using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace RythhmMagic
{
    public class BtnPlay : MonoBehaviour
    {
        [SerializeField] bool autoPlay;
        RythmMagicMain main;
        private void Start()
        {
            main = FindObjectOfType<RythmMagicMain>();
        }

        private void Update()
        {
            if (autoPlay)
            {
                StartGame();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.GetComponent<MarkerController>() != null &&
                other.gameObject.GetComponent<MarkerController>().CurrentHand.IsGrabbingWithType(GrabTypes.Pinch))
            {
                StartGame();
            }
        }
        void StartGame()
        {
            main.StartGame();
            Destroy(gameObject);
        }
    }
}