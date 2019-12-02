using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RyhthmCircle
{
	public class AIMarker : MonoBehaviour
	{
		// Start is called before the first frame update
		void Awake()
		{
			transform.localScale = Vector3.zero;
		}

		public void Init(float moveDuration)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 10);
			transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 361));
			DOTween.To(() => transform.localPosition, x => transform.localPosition = x, new Vector3(transform.localPosition.x, transform.localPosition.y, 0),
				moveDuration).SetEase(Ease.Linear);

			transform.DOScale(Vector3.one, moveDuration).SetEase(Ease.Linear);
		}

		// Update is called once per frame
		void Update()
		{
			if (transform.localPosition.z <= 0) Destroy(gameObject, 0.3f);
		}
	}
}