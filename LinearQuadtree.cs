using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Work in progress, trying to do something similar to:
// https://www.researchgate.net/publication/331761994_Quadtrees_on_the_GPU
public class LinearQuadtree : MonoBehaviour
{
	public Transform Source;

	float _MinX, _MaxX, _MinY, _MaxY = 0f;
	Bounds _Bounds;

	public struct Leaf
	{
		public Vector2 Location;
		public uint Address;
	};

	// Write two bits (0 to 3 in decimal) to a four-bytes unsigned int number at the specified index.
	// Index must have values from 0 to 15 (every index is ordered from right to left).
	uint WriteTwoBitsToUint(uint bits, uint u32, int index)
	{
		uint mask = 3u << (index << 1);
		return (u32 & ~mask) | ((bits & 3u) << (index << 1));
	}

	// Read two bits (0 to 3 in decimal) from a four-byte unsigned int number at the specified index.
	// Index must have values from 0 to 15 (every index is ordered from right to left).
	uint ReadTwoBitsFromUint(uint u32, int index)
	{
		uint mask = 3u << (index << 1);
		return (u32 & mask) >> (index << 1);
	}

	uint GetQuadrant(Vector2 location, float minX, float maxX, float minY, float maxY)
	{
		float midX = (minX + maxX) / 2.0f;
		float midY = (minY + maxY) / 2.0f;
		bool topLeft = location.x < midX && location.y >= midY;
		bool topRight = location.x >= midX && location.y >= midY;
		bool bottomLeft = location.x < midX && location.y < midY;
		return topLeft ? 0u : topRight ? 1u : bottomLeft ? 2u : 3u;
	}

	uint GetAddress(Vector2 location, float minX, float maxX, float minY, float maxY)
	{
		uint address = 0u;
		for (int i = 15; i >= 0; i--)
		{
			uint quadrant = GetQuadrant(location, minX, maxX, minY, maxY);
			address = WriteTwoBitsToUint(quadrant, address, i);
			float midX = (minX + maxX) / 2.0f;
			float midY = (minY + maxY) / 2.0f;
			minX = (quadrant == 1u || quadrant == 3u) ? midX : minX;
			maxX = (quadrant == 0u || quadrant == 2u) ? midX : maxX;
			minY = (quadrant == 0u || quadrant == 1u) ? midY : minY;
			maxY = (quadrant == 2u || quadrant == 3u) ? midY : maxY;
		}
		return address;
	}

	void Start()
	{
		GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		_Bounds = plane.GetComponent<Renderer>().bounds;
		_MinX = _Bounds.min.x;
		_MaxX = _Bounds.max.x;
		_MinY = _Bounds.min.z;
		_MaxY = _Bounds.max.z;
	}

	void Update()
	{
		Vector2 point = new Vector2(Source.position.x, Source.position.z);
		uint u32 = GetAddress(point, _MinX, _MaxX, _MinY, _MaxY);
		Debug.Log(point + "   " + u32 + "   " + System.Convert.ToString(u32, 2).PadLeft(32, '0'));
	}
}
