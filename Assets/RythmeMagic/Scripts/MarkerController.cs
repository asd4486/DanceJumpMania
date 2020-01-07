using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


namespace RythhmMagic
{
    public class MarkerController : MonoBehaviour
    {
        Collider myCol;
        [SerializeField] Hand currentHand;
        public Hand CurrentHand { get { return currentHand; } }

        public MarkerType controlMarkerType { get; private set; }

        SpriteRenderer spRenderer;
        [SerializeField] Material defaultMat;
        [SerializeField] Material triggerMat;

        private void Awake()
        {
            myCol = GetComponent<Collider>();
            spRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = new Vector3(currentHand.transform.position.x, currentHand.transform.position.y, transform.position.z);

            if (currentHand.IsGrabbingWithType(GrabTypes.Pinch))
            {
                controlMarkerType = MarkerType.Trigger;
                spRenderer.material = triggerMat;
            }                      
            else
            {
                spRenderer.material = defaultMat;
                controlMarkerType = MarkerType.Default;
            }                        
        }
    }
}