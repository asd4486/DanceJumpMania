using DG.Tweening;
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
	public class MarkerLine : MonoBehaviour
	{
		Material lineMat;
		public GameObject prefab;
		[SerializeField] float prefabSpacing = 3;
		List<GameObject> lineArrows = new List<GameObject>();

		public VertexPath vectexPath { get; private set; }

		public void SetMaterial(Material mat)
		{
			lineMat = mat;
		}

		public void GenerateLine(Vector3[] points)
		{
			BezierPath bezierPath = new BezierPath(points);
			vectexPath = new VertexPath(bezierPath, transform);


			if (prefab != null)
			{
				float dst = prefabSpacing;
				while (dst < vectexPath.length)
				{
					Vector3 point = vectexPath.GetPointAtDistance(dst);
					Quaternion rot = vectexPath.GetRotationAtDistance(dst);
					var o = Instantiate(prefab, point, rot, transform);
					o.GetComponentInChildren<MeshRenderer>().material = lineMat;
					lineArrows.Add(o);

					dst += prefabSpacing;
				}
			}
		}

		private void Update()
		{
			//auto hide arrow when pass
			foreach (var a in lineArrows)
			{
				if (a.transform.localScale.x > 0)
				{
					if (Mathf.Abs(transform.localPosition.z) > a.transform.localPosition.z)
						a.transform.DOScale(Vector3.zero, 0.1f);
				}
			}
		}
	}
}