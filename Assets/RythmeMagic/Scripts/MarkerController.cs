using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


namespace RythhmMagic
{
    public class MarkerController : MonoBehaviour
    {
        [SerializeField] Hand currentHand;

        // Update is called once per frame
        void Update()
        {
            transform.position = new Vector3(currentHand.transform.position.x, currentHand.transform.position.y, transform.position.z);
        }

        private void OnCollisionStay(Collision collision)
        {
            Debug.Log("touch");
            if (collision.gameObject.GetComponent<AIMarker>() != null)
            {
                collision.gameObject.GetComponent<AIMarker>().OnTouchMarker();
            }
        }

    }
}