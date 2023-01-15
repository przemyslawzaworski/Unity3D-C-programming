// https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class DelaunayTriangulation : MonoBehaviour
{
	public int VertexCount = 128;
	Material _Material;
	List<Triangle> _Triangles = new List<Triangle>();

	struct Edge
	{
		public Vector2 a;
		public Vector2 b;
	 
		public Edge(Vector2 a, Vector2 b)
		{
			this.a = a;
			this.b = b;
		}
	}

	struct Triangle
	{
		public Vector2 a;
		public Vector2 b;
		public Vector2 c;
	
		public Triangle(Vector2 a, Vector2 b, Vector2 c)
		{
			this.a = a;
			this.b = b;
			this.c = c;
		}
	
		public Vector2 this[int i]
		{
			get
			{
				switch (i)
				{
					case 0:
						return a;
					case 1:
						return b;
					case 2:
						return c;
					default:
						throw new System.ArgumentOutOfRangeException();
				}
			}
		}
	}

	bool IsEdgesEqual(Edge p, Edge q)
	{
		bool x = Mathf.Abs(p.a.x * p.b.x - q.a.x * q.b.x) < 0.00001f;
		bool y = Mathf.Abs(p.a.y * p.b.y - q.a.y * q.b.y) < 0.00001f;
		return x && y;
	}

	bool IsTrianglesEqual(Triangle p, Triangle q)
	{
		bool x = Mathf.Abs(p.a.x * p.b.x * p.c.x - q.a.x * q.b.x * q.c.x) < 0.00001f;
		bool y = Mathf.Abs(p.a.y * p.b.y * p.c.y - q.a.y * q.b.y * q.c.y) < 0.00001f;
		return x && y;
	}

	bool IsPointInCircumcircleOfTriangle (Vector2 p, Triangle t)
	{
		float ax = t.b.x - t.a.x;
		float ay = t.b.y - t.a.y;
		float bx = t.c.x - t.a.x;
		float by = t.c.y - t.a.y;
		float m = t.b.x * t.b.x - t.a.x * t.a.x + t.b.y * t.b.y - t.a.y * t.a.y;
		float u = t.c.x * t.c.x - t.a.x * t.a.x + t.c.y * t.c.y - t.a.y * t.a.y;
		float s = 1.0f / (2.0f * (ax * by - ay * bx));
		float cx = ((t.c.y - t.a.y) * m + (t.a.y - t.b.y) * u) * s;
		float cy = ((t.a.x - t.c.x) * m + (t.b.x - t.a.x) * u) * s;
		float dx = t.a.x - cx;
		float dy = t.a.y - cy;
		float radius = dx * dx + dy * dy;
		float distance = (cx - p.x) * (cx - p.x) + (cy - p.y) * (cy - p.y);
		return (distance - radius) <= 0.00001f;  // removed square root
	}

	Triangle CreateSuperTriangle(List<Vector2> points, out Vector4 bounds)
	{
		float minX = 1e9f;
		float minY = 1e9f;
		float maxX = 1e-9f;
		float maxY = 1e-9f;
		for (int i = 0; i < points.Count; i++)
		{
			Vector2 p = points[i];
			minX = Mathf.Min(minX, p.x);
			maxX = Mathf.Max(maxX, p.x);
			minY = Mathf.Min(minY, p.y);
			maxY = Mathf.Max(maxY, p.y);
		}
		bounds = new Vector4(minX, minY, maxX, maxY);
		float dmax = Mathf.Max(maxX - minX, maxY - minY);
		float xmid = (minX + maxX) * 0.5f;
		float ymid = (minY + maxY) * 0.5f;
		Vector2 p1 = new Vector2(xmid - 20f * dmax, ymid - dmax);
		Vector2 p2 = new Vector2(xmid, ymid + 20f * dmax);
		Vector2 p3 = new Vector2(xmid + 20f * dmax, ymid - dmax);
		return new Triangle(p1, p2, p3);
	}

	List<Triangle> Triangulation(List<Vector2> pointList)
	{   // pointList is a set of coordinates defining the points to be triangulated
		List<Triangle> triangulation = new List<Triangle>();
		Triangle superTriangle = CreateSuperTriangle(pointList, out Vector4 bounds);
		triangulation.Add(superTriangle);
		for (int i = 0; i < pointList.Count; i++)
		{   // add all the points one at a time to the triangulation
			Vector2 p = pointList[i];
			List<Triangle> badTriangles = new List<Triangle>();
			for (int j = 0; j < triangulation.Count; j++)
			{   // first find all the triangles that are no longer valid due to the insertion
				Triangle t = triangulation[j];
				if (IsPointInCircumcircleOfTriangle(p, t))
				{
					badTriangles.Add(t);
				}
			}
			List<Edge> polygon = new List<Edge>();
			for (int j = 0; j < badTriangles.Count; j++)
			{   // find the boundary of the polygonal hole
				Triangle t = badTriangles[j];
				for (int k = 0; k < 3; k++)
				{
					Edge e = new Edge(t[k], t[(k + 1) % 3]);
					List<Edge> current = new List<Edge>();
					for (int m = 0; m < badTriangles.Count; m++)
					{
						if (m == j) continue;
						Triangle tr = badTriangles[m];
						for (int q = 0; q < 3; q++)
						current.Add(new Edge(tr[q], tr[(q + 1) % 3]));
					}
					bool contains = false;
					for (int n = 0; n < current.Count; n++)
					{
						if (IsEdgesEqual(current[n], e)) contains = true;
					}
					if (contains == false)
					{
						polygon.Add(e);
					}
				}
			}
			for (int j = 0; j < badTriangles.Count; j++)
			{   // remove them from the data structure
				for(int m = triangulation.Count - 1; m > -1; m--)
				{
					if(IsTrianglesEqual(triangulation[m], badTriangles[j]))
					{
						triangulation.RemoveAt(m);
					}
				}
			}
			for (int j = 0; j < polygon.Count; j++)
			{   // re-triangulate the polygonal hole
				Edge e = polygon[j];
				Triangle t = new Triangle(p, e.a, e.b);
				triangulation.Add(t);
			}
		}
		for (int i = triangulation.Count - 1; i > -1; i--)
		{   // done inserting points, now clean up
			bool ax = triangulation[i].a.x < bounds.x || triangulation[i].a.x > bounds.z;
			bool ay = triangulation[i].a.y < bounds.y || triangulation[i].a.y > bounds.w;
			bool bx = triangulation[i].b.x < bounds.x || triangulation[i].b.x > bounds.z;
			bool by = triangulation[i].b.y < bounds.y || triangulation[i].b.y > bounds.w;
			bool cx = triangulation[i].c.x < bounds.x || triangulation[i].c.x > bounds.z;
			bool cy = triangulation[i].c.y < bounds.y || triangulation[i].c.y > bounds.w;
			if (ax || ay || bx || by || cx || cy) triangulation.RemoveAt(i);
		}
		return triangulation;
	}

	void Generate()
	{
		List<Vector2> points = new List<Vector2>();
		for (int i = 0; i < VertexCount; i++)
		{
			points.Add(Random.insideUnitCircle * new Vector2(-5f, 5f));
		}
		_Triangles = Triangulation(points);
	}

	void Start()
	{
		_Material = new Material(Shader.Find("Hidden/Internal-Colored"));
		Generate();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Generate();
		}
	}

	void OnRenderObject()
	{
		GL.PushMatrix();
		_Material.SetPass(0);
		GL.Begin(GL.LINES);
		for (int i = 0; i < _Triangles.Count; i++)
		{
			Triangle triangle = _Triangles[i];
			GL.Vertex3(triangle.a.x, triangle.a.y, 0);
			GL.Vertex3(triangle.b.x, triangle.b.y, 0);
			GL.Vertex3(triangle.b.x, triangle.b.y, 0);
			GL.Vertex3(triangle.c.x, triangle.c.y, 0);
			GL.Vertex3(triangle.c.x, triangle.c.y, 0);
			GL.Vertex3(triangle.a.x, triangle.a.y, 0);
		}
		GL.End();
		GL.PopMatrix();
	}

	void OnDestroy()
	{
		Destroy(_Material);
	}
}