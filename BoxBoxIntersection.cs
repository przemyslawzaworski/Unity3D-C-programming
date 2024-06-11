/*
Reference: Gabor Szauer "Game Physics Cookbook"
Translated from C++ to C# by Przemyslaw Zaworski
Script checks intersection between two Oriented Bounding Boxes (OBB),
using Separating Axis Theorem (SAT).
The SAT is a technique for solving convex polygon collision problems.  
The Theorem postulates if a line can be drawn between two convex
polygons the polyhedra are not colliding. 
It is a good solution to narrow phase collision detection.
*/ 

using UnityEngine;

public class BoxBoxIntersection : MonoBehaviour
{
	public Transform CubeA;
	public Transform CubeB;

	public struct Box
	{
		public Vector3 Position;
		public Vector3 HalfSize;
		public Matrix3x3 Rotation;
		public Box(Vector3 position, Vector3 halfSize, Matrix3x3 rotation)
		{
			Position = position;
			HalfSize = halfSize;
			Rotation = rotation;
		}
	}

	public struct Matrix3x3
	{
		public Vector3 Column0;
		public Vector3 Column1;
		public Vector3 Column2;
		public Matrix3x3(Vector3 column0, Vector3 column1, Vector3 column2)
		{
			Column0 = column0;
			Column1 = column1;
			Column2 = column2;
		}
	}

	Vector2 GetInterval(Box box, Vector3 axis)
	{
		Vector3[] vertices = new Vector3[8];
		Vector3 p = box.Position;
		Vector3 e = box.HalfSize;
		Matrix3x3 m = box.Rotation;
		Vector3[] a = {m.Column0, m.Column1, m.Column2};
		vertices[0] = p + a[0] * e.x + a[1] * e.y + a[2] * e.z;
		vertices[1] = p - a[0] * e.x + a[1] * e.y + a[2] * e.z;
		vertices[2] = p + a[0] * e.x - a[1] * e.y + a[2] * e.z;
		vertices[3] = p + a[0] * e.x + a[1] * e.y - a[2] * e.z;
		vertices[4] = p - a[0] * e.x - a[1] * e.y - a[2] * e.z;
		vertices[5] = p + a[0] * e.x - a[1] * e.y - a[2] * e.z;
		vertices[6] = p - a[0] * e.x + a[1] * e.y - a[2] * e.z;
		vertices[7] = p - a[0] * e.x - a[1] * e.y + a[2] * e.z;
		Vector2 result;
		result.x = result.y = Vector3.Dot(axis, vertices[0]);
		for (int i = 1; i < 8; ++i)
		{
			float projection = Vector3.Dot(axis, vertices[i]);
			result.x = Mathf.Min(result.x, projection);
			result.y = Mathf.Max(result.y, projection);
		}
		return result;
	}

	bool Intersection(Box box1, Box box2)
	{
		Vector3[] axes = new Vector3[15];
		Matrix3x3 m1 = box1.Rotation;
		Matrix3x3 m2 = box2.Rotation;
		axes[0]  = m1.Column0;
		axes[1]  = m1.Column1;
		axes[2]  = m1.Column2;
		axes[3]  = m2.Column0;
		axes[4]  = m2.Column1;
		axes[5]  = m2.Column2;
		axes[6]  = Vector3.Cross(axes[0], axes[3]);
		axes[7]  = Vector3.Cross(axes[0], axes[4]);
		axes[8]  = Vector3.Cross(axes[0], axes[5]);
		axes[9]  = Vector3.Cross(axes[1], axes[3]);
		axes[10] = Vector3.Cross(axes[1], axes[4]);
		axes[11] = Vector3.Cross(axes[1], axes[5]);	
		axes[12] = Vector3.Cross(axes[2], axes[3]);
		axes[13] = Vector3.Cross(axes[2], axes[4]);
		axes[14] = Vector3.Cross(axes[2], axes[5]);
		for (int i = 0; i < 15; ++i)
		{
			Vector2 a = GetInterval(box1, axes[i]);
			Vector2 b = GetInterval(box2, axes[i]);
			bool overlapOnAxis = ((b.x <= a.y) && (a.x <= b.y));
			if (overlapOnAxis == false)
			{
				return false;
			}
		}
		return true;
	}

	Box GetBoxFromTransform(Transform transform)
	{
		Vector3 position = transform.position;
		Vector3 size = transform.localScale * 0.5f;
		Quaternion quaternion = transform.rotation;
		Vector3 right = quaternion * Vector3.right;
		Vector3 up = quaternion * Vector3.up;
		Vector3 forward = quaternion * Vector3.forward;
		Matrix3x3 rotation = new Matrix3x3(right, up, forward);
		return new Box(position, size, rotation);
	}

	void Update()
	{
		Box box1 = GetBoxFromTransform(CubeA);
		Box box2 = GetBoxFromTransform(CubeB);
		bool intersects = Intersection(box1, box2);
		Debug.Log("Box intersection: " + intersects);
	}
}