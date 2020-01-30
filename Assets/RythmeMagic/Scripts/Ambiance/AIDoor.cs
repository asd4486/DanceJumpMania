using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
	public class AIDoor : MonoBehaviour
	{
		float speed;

		public void Init(float _zPos, float _speed)
		{
			transform.localPosition = new Vector3(0, 0, _zPos);
			transform.localScale = Vector3.zero;
			speed = _speed;
		}

		private void Update()
		{
			transform.localPosition -= Vector3.forward * speed * Time.deltaTime;
		}
	}
}
