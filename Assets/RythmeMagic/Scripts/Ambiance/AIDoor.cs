using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
    public class AIDoor : MonoBehaviour
    {
        [SerializeField] Material[] mats;
        float speed;
        float scaleSpeed;

        bool startMove;

        private void Start()
        {
            GetComponentInChildren<MeshRenderer>().material = mats[Random.Range(0, mats.Length)];
        }

        public void Init(float _zPos, float _speed, float time)
        {
            transform.localPosition = new Vector3(0, 0, _zPos);
            transform.localScale = Vector3.zero;
            scaleSpeed = 1 / time;
            speed = _speed;

            startMove = true;
        }

        private void Update()
        {
            if (!startMove)
                return;

            //scale for make a profond effect
            if (transform.localPosition.z > 0)
            {
                transform.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
            }
            else
            {
                transform.localScale -= Vector3.one * scaleSpeed * Time.deltaTime;
                if (transform.localScale.x <= 0)
                    Destroy(gameObject);
            }

            transform.localPosition -= Vector3.forward * speed * Time.deltaTime;
        }
    }
}
