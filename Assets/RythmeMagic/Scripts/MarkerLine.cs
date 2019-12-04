using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
	public class MarkerLine : MonoBehaviour
	{
		[SerializeField] float width = .4f;
		//[Range(0, .5f)]
		[SerializeField] float thickness = .15f;
		[SerializeField] bool flattenSurface;

		[SerializeField] Material lineMat;
		[SerializeField] float textureTiling = 1;

		public VertexPath vectexPath { get; private set; }

		MeshFilter meshFilter;
		MeshRenderer meshRenderer;
		Mesh mesh;

		private void Awake()
		{
			AssignMeshComponents();
			AssignMaterials();
		}

		void AssignMeshComponents()
		{
			// Ensure mesh renderer and filter components are assigned
			if (!gameObject.GetComponent<MeshFilter>())
				gameObject.AddComponent<MeshFilter>();

			if (!GetComponent<MeshRenderer>())
				gameObject.AddComponent<MeshRenderer>();

			meshRenderer = GetComponent<MeshRenderer>();
			meshFilter = GetComponent<MeshFilter>();
			if (mesh == null)
				mesh = new Mesh();

			meshFilter.sharedMesh = mesh;
		}

		void AssignMaterials()
		{
			if (lineMat != null)
			{
				meshRenderer.sharedMaterials = new Material[] { lineMat, lineMat, lineMat };
				meshRenderer.sharedMaterials[0].mainTextureScale = new Vector3(1, textureTiling);
			}
		}

		public void GenerateMesh(Vector3[] points)
		{
			BezierPath bezierPath = new BezierPath(points);
			vectexPath = new VertexPath(bezierPath, transform);

			Vector3[] verts = new Vector3[vectexPath.NumPoints * 8];
			Vector2[] uvs = new Vector2[verts.Length];
			Vector3[] normals = new Vector3[verts.Length];

			int numTris = 2 * (vectexPath.NumPoints - 1) + ((vectexPath.isClosedLoop) ? 2 : 0);
			int[] roadTriangles = new int[numTris * 3];
			int[] underRoadTriangles = new int[numTris * 3];
			int[] sideOfRoadTriangles = new int[numTris * 2 * 3];

			int vertIndex = 0;
			int triIndex = 0;

			// Vertices for the top of the road are layed out:
			// 0  1
			// 8  9
			// and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
			int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
			int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

			bool usePathNormals = !(vectexPath.space == PathSpace.xyz && flattenSurface);

			for (int i = 0; i < vectexPath.NumPoints; i++)
			{
				Vector3 localUp = ((usePathNormals) ? Vector3.Cross(vectexPath.GetTangent(i), vectexPath.GetNormal(i)) : vectexPath.up) / 2;
				Vector3 localRight = (usePathNormals) ? vectexPath.GetNormal(i) : Vector3.Cross(localUp, vectexPath.GetTangent(i));

				// Find position to left and right of current path vertex
				Vector3 vertSideA = vectexPath.GetPoint(i) - localRight * Mathf.Abs(width);
				Vector3 vertSideB = vectexPath.GetPoint(i) + localRight * Mathf.Abs(width);

				// Add top of road vertices
				verts[vertIndex + 0] = vertSideA + localUp * thickness;
				verts[vertIndex + 1] = vertSideB + localUp * thickness;
				// Add bottom of road vertices
				verts[vertIndex + 2] = vertSideA - localUp * thickness;
				verts[vertIndex + 3] = vertSideB - localUp * thickness;

				// Duplicate vertices to get flat shading for sides of road
				verts[vertIndex + 4] = verts[vertIndex + 0];
				verts[vertIndex + 5] = verts[vertIndex + 1];
				verts[vertIndex + 6] = verts[vertIndex + 2];
				verts[vertIndex + 7] = verts[vertIndex + 3];

				// Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
				uvs[vertIndex + 0] = new Vector2(0, vectexPath.times[i]);
				uvs[vertIndex + 1] = new Vector2(1, vectexPath.times[i]);

				// Top of road normals
				normals[vertIndex + 0] = localUp;
				normals[vertIndex + 1] = localUp;
				// Bottom of road normals
				normals[vertIndex + 2] = -localUp;
				normals[vertIndex + 3] = -localUp;
				// Sides of road normals
				normals[vertIndex + 4] = -localRight;
				normals[vertIndex + 5] = localRight;
				normals[vertIndex + 6] = -localRight;
				normals[vertIndex + 7] = localRight;

				// Set triangle indices
				if (i < vectexPath.NumPoints - 1 || vectexPath.isClosedLoop)
				{
					for (int j = 0; j < triangleMap.Length; j++)
					{
						roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
						// reverse triangle map for under road so that triangles wind the other way and are visible from underneath
						underRoadTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
					}
					for (int j = 0; j < sidesTriangleMap.Length; j++)
					{
						sideOfRoadTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
					}

				}

				vertIndex += 8;
				triIndex += 6;
			}

			mesh.Clear();
			mesh.vertices = verts;
			mesh.uv = uvs;
			mesh.normals = normals;
			mesh.subMeshCount = 3;
			mesh.SetTriangles(roadTriangles, 0);
			mesh.SetTriangles(underRoadTriangles, 1);
			mesh.SetTriangles(sideOfRoadTriangles, 2);
			mesh.RecalculateBounds();
		}
	}
}