using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To give proper results, mesh must be closed (have volume) !
public class PointInMesh : MonoBehaviour
{
	public Transform Source; // input point
	public Transform Destination; // destination mesh transform

	private Vector3[] _Vertices;
	private int[] _Triangles;

	// ro = ray origin
	// rd = ray direction
	// a,b,c = triangle vertices in world space
	// hit = optionally get ray hit position in world space
	bool RayTriangleIntersection(Vector3 ro, Vector3 rd, Vector3 a, Vector3 b, Vector3 c, out Vector3 hit)
	{
		hit = new Vector3(0f, 0f, 0f);
		Vector3 ba = b - a;
		Vector3 ca = c - a;
		Vector3 p = Vector3.Cross(rd, ca);
		float det = Vector3.Dot(ba, p);
		if (Mathf.Abs(det) < 1e-5f) return false;
		float invDet = 1.0f / det;
		Vector3 t = ro - a;
		float u = Vector3.Dot(t, p) * invDet;
		if (u < 0f || u > 1f) return false;
		Vector3 q = Vector3.Cross(t, ba);
		float v = Vector3.Dot(rd, q) * invDet;
		if (v < 0f || u + v > 1f) return false;
		hit = a + u * ba + v * ca;
		if ((Vector3.Dot(ca, q) * invDet) > 1e-5f) return true;
		return false;
	}

	// position = input point in world space
	// transform = mesh transform
	// vertices = mesh vertices
	// triangles = mesh triangles (indices)
	bool IsPointInsideMesh(Vector3 position, Transform transform, Vector3[] vertices, int[] triangles)
	{
		Vector3 direction = Vector3.Normalize(Random.insideUnitSphere + Vector3.one);
		int intersections = 0;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			Vector3 a = transform.TransformPoint(vertices[triangles[i + 0]]);
			Vector3 b = transform.TransformPoint(vertices[triangles[i + 1]]);
			Vector3 c = transform.TransformPoint(vertices[triangles[i + 2]]);
			intersections += RayTriangleIntersection(position, direction, a, b, c, out Vector3 hit) ? 1 : 0;
		}
		return (intersections % 2 == 1);
	}

	void Start()
	{
		Mesh mesh = Destination.gameObject.GetComponent<MeshFilter>().sharedMesh;
		_Vertices = new List<Vector3>(mesh.vertices).ToArray();
		_Triangles = new List<int>(mesh.triangles).ToArray();
	}

	void Update()
	{
		Debug.Log(IsPointInsideMesh(Source.position, Destination, _Vertices, _Triangles));
	}
}