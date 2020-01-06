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

        private void Awake()
        {
            myCol = GetComponent<Collider>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = new Vector3(currentHand.transform.position.x, currentHand.transform.position.y, transform.position.z);

            //myCol.enabled = currentHand.IsGrabbingWithType(GrabTypes.Pinch);
        }
    }
}