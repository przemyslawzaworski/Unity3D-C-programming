using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mercator : MonoBehaviour
{
	// https://en.wikipedia.org/wiki/Spherical_coordinate_system
	Vector3 CartesianToSpherical (Vector3 p)
	{
		float radius = Mathf.Sqrt(p.x * p.x + p.y * p.y + p.z * p.z);
		float theta = Mathf.Acos(p.z / radius);
		float phi = Mathf.Atan2(p.y, p.x);
		return new Vector3(radius, theta, phi);
	}

	// https://en.wikipedia.org/wiki/Spherical_coordinate_system
	Vector3 SphericalToCartesian (Vector3 p)
	{
		float x = p.x * Mathf.Cos(p.z) * Mathf.Sin(p.y);
		float y = p.x * Mathf.Sin(p.z) * Mathf.Sin(p.y);
		float z = p.x * Mathf.Cos(p.y);
		return new Vector3(x, y, z);
	}

	// https://en.wikipedia.org/wiki/Mercator_projection
	Vector3 SphericalToMercator (Vector3 p)
	{
		float x = p.x * p.y;
		float y = p.x * Mathf.Log(Mathf.Abs(Mathf.Tan((Mathf.PI / 4.0f) + (p.z / 2.0f))));
		float z = p.x;
		return new Vector3(x, y, z);
	}

	// https://en.wikipedia.org/wiki/Mercator_projection
	Vector3 MercatorToSpherical (Vector3 p)
	{
		float radius = p.z;
		float theta = p.x / p.z;
		float phi = 2.0f * Mathf.Atan(Mathf.Exp(p.y / p.z)) - Mathf.PI / 2.0f;
		return new Vector3(radius, theta, phi);
	}
}