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
                float dst = 0;

                while (dst < vectexPath.length)
                {
                    Vector3 point = vectexPath.GetPointAtDistance(dst);

                    var o = Instantiate(prefab, point, Quaternion.identity, transform);
                    o.GetComponent<MeshRenderer>().material = lineMat;

                    dst += prefabSpacing;
                }
            }
        }
    }
}