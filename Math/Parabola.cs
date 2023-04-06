using System.Collections.Generic;
using UnityEngine;

namespace QuadraticFunctions
{
	public class Parabola : MonoBehaviour
	{
		public Transform Source;
		public Transform Destination;
		public DirectionType DirectionMode = DirectionType.Angle;
		public HeightType HeightMode = HeightType.Relative;
		public float Angle = 45f;
		public float Height = 5.0f;
		public float Radius = 0.25f;
		public int Steps = 128;	

		private List<Vector3> _Points;

		public enum DirectionType {Angle, Height}
		public enum HeightType {Absolute, Relative}

		float[,] Inverse3x3(float[,] matrix)
		{
			float[,] result = new float[3, 3];
			float det = matrix[0, 0] * (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1])
					  - matrix[0, 1] * (matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0])
					  + matrix[0, 2] * (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]);
			float invDet = 1.0f / det;
			result[0, 0] =  (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1]) * invDet;
			result[0, 1] = -(matrix[0, 1] * matrix[2, 2] - matrix[0, 2] * matrix[2, 1]) * invDet;
			result[0, 2] =  (matrix[0, 1] * matrix[1, 2] - matrix[0, 2] * matrix[1, 1]) * invDet;
			result[1, 0] = -(matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0]) * invDet;
			result[1, 1] =  (matrix[0, 0] * matrix[2, 2] - matrix[0, 2] * matrix[2, 0]) * invDet;
			result[1, 2] = -(matrix[0, 0] * matrix[1, 2] - matrix[0, 2] * matrix[1, 0]) * invDet;
			result[2, 0] =  (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]) * invDet;
			result[2, 1] = -(matrix[0, 0] * matrix[2, 1] - matrix[0, 1] * matrix[2, 0]) * invDet;
			result[2, 2] =  (matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0]) * invDet;
			return result;
		}

		float[] Multiply3x3(float[,] matrix, float[] vector)
		{
			float[] result = new float[3];
			for (int i = 0; i < 3; i++)
			{
				result[i] = matrix[i, 0] * vector[0] + matrix[i, 1] * vector[1] + matrix[i, 2] * vector[2];
			}
			return result;
		}

		Vector3 GetCoefficients(float x1, float y1, float x2, float y2, float x3, float y3)
		{
			float[,] matrix = new float[3, 3]
			{
				{x1 * x1, x1, 1f},
				{x2 * x2, x2, 1f},
				{x3 * x3, x3, 1f}
			};
			float[,] inverse = Inverse3x3(matrix);
			float[] result = Multiply3x3(inverse, new float[] {y1, y2, y3});
			return new Vector3(result[0], result[1], result[2]);
		}

		void Evaluation()
		{
			_Points = new List<Vector3>();
			Vector2 src = new Vector2(Source.position.x, Source.position.z);
			Vector2 dst = new Vector2(Destination.position.x, Destination.position.z);
			float u = 0.0f;
			float v = Vector2.Distance(dst, src);
			float x1 = u;
			float y1 = Source.position.y;
			float x2 = Mathf.Lerp(u, v, 0.5f);
			if (DirectionMode == DirectionType.Angle) Height = Mathf.Tan(Angle * Mathf.Deg2Rad) * x2;
			if (HeightMode == HeightType.Relative) Height += Mathf.Lerp(Source.position.y, Destination.position.y, 0.5f);
			float y2 = Height;
			float x3 = v;
			float y3 = Destination.position.y;
			Vector3 abc = GetCoefficients(x1, y1, x2, y2, x3, y3);
			float a = abc.x;
			float b = abc.y;
			float c = abc.z;
			for (int i = 0; i < Steps; i++)
			{
				float x = Mathf.Lerp(u, v, (float)i / (float)Steps);
				float y = a * x * x + b * x + c;
				Vector2 result = Vector2.Lerp(src, dst, x / v);
				Vector3 position = new Vector3(result.x, y, result.y);
				_Points.Add(position);
			}
		}

		void OnDrawGizmos()
		{
			Evaluation();
			for (int i = 0; i < _Points.Count; i++) Gizmos.DrawSphere(_Points[i], Radius);
		}
	}
}