using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace RythhmMagic
{
    [RequireComponent(typeof(BoxCollider))]
    public class ControllerLaser : MonoBehaviour
    {
        LineRenderer line;
        [SerializeField] MarkerController controller;
        [SerializeField] Transform gameZone;

        public event PointerEventHandler PointerIn;
        public event PointerEventHandler PointerOut;

        bool pointerClicked;
        public event PointerEventHandler PointerClick;

        Transform previousContact = null;

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = controller.CurrentHand.transform.position;
            line.SetPosition(1, new Vector3(0, 0, Mathf.Abs(gameZone.transform.position.z - controller.CurrentHand.transform.position.z)));
            line.material = controller.currentMat;


            float dist = 100f;

            RaycastHit hit;
            bool bHit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity);

            if (previousContact && previousContact != hit.transform)
            {
                PointerEventArgs args = new PointerEventArgs();
                args.distance = 0f;
                args.flags = 0;
                args.target = previousContact;
                OnPointerOut(args);
                previousContact = null;
            }
            if (bHit && previousContact != hit.transform)
            {
                PointerEventArgs argsIn = new PointerEventArgs();
                argsIn.distance = hit.distance;
                argsIn.flags = 0;
                argsIn.target = hit.transform;
                OnPointerIn(argsIn);
                previousContact = hit.transform;   
            }
            if (!bHit)
            {
                previousContact = null;
            }
            if (bHit && hit.distance < 100f)
            {
                dist = hit.distance;
            }

            if (bHit && controller.TriggerDown && !pointerClicked)
            {
                pointerClicked = true;

                PointerEventArgs argsClick = new PointerEventArgs();
                argsClick.distance = hit.distance;
                argsClick.flags = 0;
                argsClick.target = hit.transform;
                OnPointerClick(argsClick);
            }

            //avoid repeat action
            if (pointerClicked && !controller.TriggerDown)
                pointerClicked = false;
        }

        private void OnPointerIn(PointerEventArgs e)
        {
            if (PointerIn != null)
                PointerIn(this, e);
        }

        private void OnPointerClick(PointerEventArgs e)
        {
            if (PointerClick != null)
                PointerClick(this, e);
        }

        private void OnPointerOut(PointerEventArgs e)
        {
            if (PointerOut != null)
                PointerOut(this, e);
        }
    }

    public struct PointerEventArgs
    {
        public uint flags;
        public float distance;
        public Transform target;
    }

    public delegate void PointerEventHandler(object sender, PointerEventArgs e);
}
