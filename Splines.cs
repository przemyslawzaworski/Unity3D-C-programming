using UnityEngine;

public class Splines : MonoBehaviour
{
	/// <summary>
	/// Returns a point on the Catmull–Rom spline defined by p0→p1→p2→p3 at parameter t∈[0,1].
	/// </summary>
	Vector2 CatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) 
	{
		Vector2 a =  2f * p1;
		Vector2 b =  p2 - p0;
		Vector2 c =  2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector2 d = -p0 + 3f * p1 - 3f * p2 + p3;
		return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
	}

	/// <summary>
	/// Interpolates a point on a Hermite spline defined by four control points.
	/// </summary>
	/// <param name="p0">Control point before the start.</param>
	/// <param name="p1">Start control point.</param>
	/// <param name="p2">End control point.</param>
	/// <param name="p3">Control point after the end.</param>
	/// <param name="t">Interpolation factor (0 to 1).</param>
	/// <returns>Point on the spline between p1 and p2.</returns>
	Vector2 CubicHermite(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
	{
		Vector2 m1 = 0.5f * (p2 - p0);
		Vector2 m2 = 0.5f * (p3 - p1);
		float t2 = t * t;
		float t3 = t2 * t;
		float h00 = 2f * t3 - 3f * t2 + 1f;
		float h10 = t3 - 2f * t2 + t;
		float h01 = -2f * t3 + 3f * t2;
		float h11 = t3 - t2;
		return h00 * p1 + h10 * m1 + h01 * p2 + h11 * m2;
	}

	/// <summary>
	/// Cubic Bézier interpolation between p0→p1→p2→p3 at parameter t (0…1).
	/// </summary>
	Vector2 BezierInterpolation(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
	{
		float t2  = t * t;
		float t3  = t2 * t;
		float mt  = 1f - t;
		float mt2 = mt * mt;
		float mt3 = mt2 * mt;
		return mt3 * p0 + 3f * mt2 * t * p1 + 3f * mt * t2 * p2 + t3 * p3;
	}

	/// <summary>
	/// Kochanek–Bartels (TCB) interpolation: p0–p1–p2–p3 control points, with tension, bias, continuity.
	/// Computes outgoing/incoming tangents and then performs the cubic Hermite blend inline.
	/// </summary>
	Vector2 KochanekBartels(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t, float tension, float bias, float continuity)
	{
		float s = 0.5f * (1f - tension);
		Vector2 d1 = s * ((1f + bias) * (1f + continuity) * (p1 - p0) + (1f - bias) * (1f - continuity) * (p2 - p1));
		Vector2 d2 = s * ((1f - bias) * (1f + continuity) * (p2 - p1) + (1f + bias) * (1f - continuity) * (p3 - p2));
		float t2 = t * t;
		float t3 = t2 * t;
		float h00 = 2f * t3 - 3f * t2 + 1f;
		float h10 = t3 - 2f * t2 + t;
		float h01 = -2f * t3 + 3f * t2;
		float h11 = t3 - t2;
		return h00 * p1 + h10 * d1 + h01 * p2 + h11 * d2;
	}

	/// <summary>
	/// Cubic B-spline interpolation of four control points.
	/// t ∈ [0,1] where 0→p1 and 1→p2.
	/// </summary>
	Vector2 BSpline(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
	{
		float t2 = t * t;
		float t3 = t2 * t;
		float b0 = (1f - t) * (1f - t) * (1f - t) / 6f;
		float b1 = (3f * t3 - 6f * t2 + 4f) / 6f;
		float b2 = (-3f * t3 + 3f * t2 + 3f * t + 1f) / 6f;
		float b3 = t3 / 6f;
		return b0 * p0 + b1 * p1 + b2 * p2 + b3 * p3;
	}

	/// <summary>
	/// Akima spline interpolation between p2 and p3, using p0…p5 as the surrounding samples.
	/// </summary>
	/// <param name="p0">Two samples before the interval</param>
	/// <param name="p1">One sample before the interval</param>
	/// <param name="p2">Start of the interval</param>
	/// <param name="p3">End of the interval</param>
	/// <param name="p4">One sample after the interval</param>
	/// <param name="p5">Two samples after the interval</param>
	/// <param name="t">Interpolation parameter (0…1)</param>
	/// <returns>Interpolated point between p2 and p3</returns>
	Vector2 Akima(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 p5, float t)
	{
		Vector2 m1 = p1 - p0;
		Vector2 m2 = p2 - p1;
		Vector2 m3 = p3 - p2;
		Vector2 m4 = p4 - p3;
		Vector2 m5 = p5 - p4;
		Vector2 m6 = m3 - m2;
		Vector2 m7 = m2 - m1;
		Vector2 m8 = m4 - m3;
		Vector2 m9 = m5 - m4;
		float w1 = Mathf.Sqrt(m6.x * m6.x + m6.y * m6.y);
		float w2 = Mathf.Sqrt(m7.x * m7.x + m7.y * m7.y);
		float w3 = Mathf.Sqrt(m8.x * m8.x + m8.y * m8.y);
		float w4 = Mathf.Sqrt(m9.x * m9.x + m9.y * m9.y);
		float eps = 0.000001f;
		Vector2 s2 = (Mathf.Abs(w1) < eps && Mathf.Abs(w2) < eps) ? (m1 + m2) * 0.5f : (w1 * m1 + w2 * m2) / (w1 + w2);
		Vector2 s3 = (Mathf.Abs(w3) < eps && Mathf.Abs(w4) < eps) ? (m3 + m4) * 0.5f : (w3 * m3 + w4 * m4) / (w3 + w4);
		float t2 = t * t;
		float t3 = t2 * t;
		float h00 = 2f * t3 - 3f * t2 + 1f;
		float h10 = t3 - 2f * t2 + t;
		float h01 = -2f * t3 + 3f * t2;
		float h11 = t3 - t2;
		return h00 * p2 + h10 * s2 + h01 * p3 + h11 * s3;
	}

	void Start()
	{
		Vector2 p0 = new Vector2(0f, 0f);
		Vector2 p1 = new Vector2(1f, 2f);
		Vector2 p2 = new Vector2(3f, 3f);
		Vector2 p3 = new Vector2(4f, 1f);
		Vector2 p4 = new Vector2(5f, 0f);
		Vector2 p5 = new Vector2(6f, 2f);
		int samples = 64;
		DrawSplineWithCubes("CatmullRom", 0, samples, (t) => CatmullRom(p0, p1, p2, p3, t));
		DrawSplineWithCubes("Hermite",    1, samples, (t) => CubicHermite(p0, p1, p2, p3, t));
		DrawSplineWithCubes("Bezier",     2, samples, (t) => BezierInterpolation(p0, p1, p2, p3, t));
		DrawSplineWithCubes("KB",         3, samples, (t) => KochanekBartels(p0, p1, p2, p3, t, 0f, 0f, 0f));
		DrawSplineWithCubes("BSpline",    4, samples, (t) => BSpline(p0, p1, p2, p3, t));
		DrawSplineWithCubes("Akima",      5, samples, (t) => Akima(p0, p1, p2, p3, p4, p5, t));
	}

	void DrawSplineWithCubes(string name, int zIndex, int samples, System.Func<float, Vector2> splineFunc)
	{
		GameObject parent = new GameObject(name + "Spline");
		for (int i = 0; i <= samples; i++)
		{
			float t = i / (float)samples;
			Vector2 point2D = splineFunc(t);
			Vector3 point3D = new Vector3(point2D.x, point2D.y, zIndex);
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.name = name + "Point_" + i;
			cube.transform.position = point3D;
			cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			cube.transform.SetParent(parent.transform);
		}
	}
}