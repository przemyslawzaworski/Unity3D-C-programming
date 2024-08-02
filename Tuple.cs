using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tuple : MonoBehaviour
{
	private (float area, float perimeter) GetTriangleInfo1(Vector3 a, Vector3 b, Vector3 c)
	{
		Vector3 ab = b - a;
		Vector3 ac = c - a;
		Vector3 crossProduct = Vector3.Cross(ab, ac);
		float area = Vector3.Magnitude(crossProduct) * 0.5f;
		float LengthAB = Vector3.Distance(a, b);
		float LengthBC = Vector3.Distance(b, c);
		float LengthCA = Vector3.Distance(c, a);
		float perimeter = LengthAB + LengthBC + LengthCA;
		return (area, perimeter);
	}

	private Tuple<float, float> GetTriangleInfo2(Vector3 a, Vector3 b, Vector3 c)
	{
		Vector3 ab = b - a;
		Vector3 ac = c - a;
		Vector3 crossProduct = Vector3.Cross(ab, ac);
		float area = Vector3.Magnitude(crossProduct) * 0.5f;
		float LengthAB = Vector3.Distance(a, b);
		float LengthBC = Vector3.Distance(b, c);
		float LengthCA = Vector3.Distance(c, a);
		float perimeter = LengthAB + LengthBC + LengthCA;
		return new Tuple<float, float>(area, perimeter);
	}

	void Start()
	{
		Vector3 A = new Vector3(0, 0, 0);
		Vector3 B = new Vector3(1, 0, 0);
		Vector3 C = new Vector3(0, 1, 0);
		(float area, float perimeter) = GetTriangleInfo1(A, B, C);
		Debug.Log($"Tuple-based method - Area: {area}, Perimeter: {perimeter}");
		Tuple<float, float> result = GetTriangleInfo2(A, B, C);
		Debug.Log($"Tuple method - Area: {result.Item1}, Perimeter: {result.Item2}");
	}
}