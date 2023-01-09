using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To give proper results, mesh must be closed (have volume) !
public class PointInMesh : MonoBehaviour
{
	[Header("Input point")]
	public Transform Source; // input point
	[Header("Destination Mesh")]
	public Transform Destination; // destination mesh transform
	[Header("Select 1 || 2 || 3 || 4")]
	public int Function = 1;

	private Vector3[] _Vertices;
	private int[] _Triangles;
	private MeshCollider _MeshCollider;

	// Möller–Trumbore ray-triangle intersection algorithm: http://www.graphics.cornell.edu/pubs/1997/MT97.pdf 
	// https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
	// ro = ray origin
	// rd = ray direction
	// a,b,c = triangle vertices in world space
	// hit = optionally get ray hit position in world space	
	bool RayTriangleIntersection (Vector3 ro, Vector3 rd, Vector3 a, Vector3 b, Vector3 c, out Vector3 hit)
	{
		float epsilon = 0.0000001f;
		hit = new Vector3(0f, 0f, 0f);
		Vector3 ba = b - a;
		Vector3 ca = c - a;
		Vector3 h = Vector3.Cross(rd, ca);
		float det = Vector3.Dot(ba, h);
		if (det > -epsilon && det < epsilon) return false;
		float f = 1.0f / det;
		Vector3 s = ro - a;
		float u = Vector3.Dot(s, h) * f;
		if (u < 0.0f || u > 1.0f) return false;
		Vector3 q = Vector3.Cross(s, ba);
		float v = Vector3.Dot(rd, q) * f;
		if (v < 0.0f || u + v > 1.0f) return false;
		float t = Vector3.Dot(ca, q) * f;
		hit = ro + rd * t;
		return (t > epsilon);
	}

	// First method:
	// Create a ray (infinite line starting at input point and going in some random direction).
	// Find intersections between ray and all mesh triangles. An odd number of intersections means it is inside the mesh.
	// position = input point in world space
	// transform = mesh transform
	// vertices = mesh vertices
	// triangles = mesh triangles (indices)
	bool IsPointInsideMesh (Vector3 position, Transform transform, Vector3[] vertices, int[] triangles)
	{
		Vector3 epsilon = new Vector3(0.001f, 0.001f, 0.001f);
		Vector3 direction = Vector3.Normalize(Random.insideUnitSphere + epsilon);
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

	// Second method:
	// Create a ray (infinite line starting at input point and going in some random direction).
	// Find the closest intersection between ray and all mesh triangles.
	// If triangle in the closest intersection is hit to back face, it means input point is inside the mesh.
	// transform = mesh transform
	// vertices = mesh vertices
	// triangles = mesh triangles (indices)	
	// position = input point in world space
	bool IsPointInsideMesh (Transform transform, Vector3[] vertices, int[] triangles, Vector3 position)
	{
		Vector3 epsilon = new Vector3(0.001f, 0.001f, 0.001f);
		Vector3 direction = Vector3.Normalize(Random.insideUnitSphere + epsilon);
		float closestDistance = 1e9f;
		bool isBackFace = false;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			Vector3 a = transform.TransformPoint(vertices[triangles[i + 0]]);
			Vector3 b = transform.TransformPoint(vertices[triangles[i + 1]]);
			Vector3 c = transform.TransformPoint(vertices[triangles[i + 2]]);
			if (RayTriangleIntersection(position, direction, a, b, c, out Vector3 hit))
			{
				float currentDistance = Vector3.Distance(hit, position);
				if (currentDistance < closestDistance)
				{
					Vector3 ba = b - a; 
					Vector3 ca = c - a;
					Vector3 triangleNormal = Vector3.Normalize(Vector3.Cross(ba, ca));
					closestDistance = currentDistance;
					isBackFace = Vector3.Dot(direction, triangleNormal) > 0f ? true : false;
				}
			}
		}
		return isBackFace;
	}

	// Third method:
	// RaycastAll returns hits of all colliders hit, but per collider only the closest one.
	// So we need to use while loop to trace current position and count number of intersections.
	// An odd number of intersections means it is inside the mesh.
	// position = input point in world space
	// collider = mesh collider
	bool IsPointInsideCollider (Vector3 position, MeshCollider collider)
	{
		Physics.queriesHitBackfaces = true;
		Vector3 epsilon = new Vector3(0.001f, 0.001f, 0.001f);
		Vector3 direction = Vector3.Normalize(Random.insideUnitSphere + epsilon);
		int intersections = 0;
		bool exit = false;
		while (!exit)
		{
			exit = true;
			RaycastHit[] hits = Physics.RaycastAll(position, direction);
			for (int i = 0; i < hits.Length; i++)
			{
				if (hits[i].collider == collider)
				{
					position = hits[i].point + direction * 0.001f;
					intersections++;
					exit = false;
					break;
				}
			}
		}
		Physics.queriesHitBackfaces = false;
		return (intersections % 2 == 1);
	}

	// Triangle Signed Distance Function
	// https://iquilezles.org/articles/distfunctions/
	float TriangleSDF (Vector3 p, Vector3 a, Vector3 b, Vector3 c)
	{
		Vector3 ba = b - a; 
		Vector3 pa = p - a;
		Vector3 cb = c - b; 
		Vector3 pb = p - b;
		Vector3 ac = a - c; 
		Vector3 pc = p - c;
		Vector3 nm = Vector3.Cross(ba, ac);
		float sa = Mathf.Sign(Vector3.Dot(Vector3.Cross(ba, nm), pa));
		float sb = Mathf.Sign(Vector3.Dot(Vector3.Cross(cb, nm), pb));
		float sc = Mathf.Sign(Vector3.Dot(Vector3.Cross(ac, nm), pc));
		Vector3 va = ba * Mathf.Clamp(Vector3.Dot(ba, pa) / Vector3.Dot(ba, ba), 0.0f, 1.0f) - pa;
		Vector3 vb = cb * Mathf.Clamp(Vector3.Dot(cb, pb) / Vector3.Dot(cb, cb), 0.0f, 1.0f) - pb;
		Vector3 vc = ac * Mathf.Clamp(Vector3.Dot(ac, pc) / Vector3.Dot(ac, ac), 0.0f, 1.0f) - pc;
		float m1 = Mathf.Min(Mathf.Min(Vector3.Dot(va, va), Vector3.Dot(vb, vb)), Vector3.Dot(vc, vc));
		float m2 = Vector3.Dot(nm, pa) * Vector3.Dot(nm, pa) / Vector3.Dot(nm, nm);
		return Mathf.Sqrt(sa + sb + sc < 2.0 ? m1 : m2);
	}

	// Fourth method:
	// Calculate signed distance field from triangle mesh.
	// Distance between point located inside mesh and the closest triangle has negative sign.
	bool IsPointInsideMesh (Vector3 position, Transform transform, Vector3[] vertices, int[] triangles, out float minimalDistance)
	{
		minimalDistance = 1e9f;
		float currentSign = 0.0f;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			Vector3 a = transform.TransformPoint(vertices[triangles[i + 0]]); // from local space to world space
			Vector3 b = transform.TransformPoint(vertices[triangles[i + 1]]); // from local space to world space
			Vector3 c = transform.TransformPoint(vertices[triangles[i + 2]]); // from local space to world space
			Vector3 ba = b - a;
			Vector3 ca = c - a;
			float currentDistance = TriangleSDF (position, a, b, c);
			if (currentDistance < minimalDistance)
			{
				Vector3 triangleNormal = Vector3.Normalize(Vector3.Cross(ba, ca));
				Vector3 triangleCenter = (a + b + c) / 3.0f;
				Vector3 direction = Vector3.Normalize(triangleCenter - position);
				currentSign = -1.0f * System.Math.Sign(Vector3.Dot(direction, triangleNormal));
				minimalDistance = currentDistance;
			}
		}
		minimalDistance = minimalDistance * currentSign;
		return currentSign < 0.0f;
	}

	void Start()
	{
		Mesh mesh = Destination.gameObject.GetComponent<MeshFilter>().sharedMesh;
		_Vertices = new List<Vector3>(mesh.vertices).ToArray();
		_Triangles = new List<int>(mesh.triangles).ToArray();
		_MeshCollider = Destination.gameObject.GetComponent<MeshCollider>();
	}

	void Update()
	{
		switch (Function) 
		{
			case 1:
				Debug.Log(IsPointInsideMesh(Source.position, Destination, _Vertices, _Triangles));
				break;
			case 2:
				Debug.Log(IsPointInsideMesh(Destination, _Vertices, _Triangles, Source.position));
				break;
			case 3:
				Debug.Log(IsPointInsideCollider (Source.position, _MeshCollider));
				break;
			case 4:
				Debug.Log(IsPointInsideMesh(Source.position, Destination, _Vertices, _Triangles, out float sdf));
				break;
		}
	}
}