using UnityEngine;

/*
PointA (Before PointB): This control point affects the direction and curvature of the spline as it approaches PointB. It pulls the spline towards itself.
PointB (Start Point): This is the starting point of the spline segment.
PointC (End Point): This is the ending point of the spline segment.
PointD (After PointC): This control point affects the direction and curvature of the spline as it departs from PointC. It pulls the spline towards itself.
http://www.mvps.org/directx/articles/catmull/
*/

public class CatmullRom : MonoBehaviour
{
	public Transform PointA;
	public Transform PointB;
	public Transform PointC;
	public Transform PointD;
	public int Segments = 64;

	private Transform[] _Segments;

	Vector3 CatmullRomSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		Vector3 a = 2f * p1;
		Vector3 b = p2 - p0;
		Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;
		return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
	}

	void Start()
	{
		_Segments = new Transform[Segments];
		for (int i = 0 ; i < Segments; i++)
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			_Segments[i] = cube.transform;
		}
	}

	void Update()
	{
		if (PointA == null || PointB == null || PointC == null || PointD == null) return;
		for (int i = 0; i < Segments; i++)
		{
			float t = (float) i / (float)(Segments - 1);
			_Segments[i].position = CatmullRomSpline(PointA.position, PointB.position, PointC.position, PointD.position, t);
		}
	}
}