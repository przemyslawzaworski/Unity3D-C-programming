using UnityEngine;

public class ParabolicMovement : MonoBehaviour
{
	public Transform Source;
	public Transform Destination;
	public Transform Bullet;
	public DirectionType DirectionMode = DirectionType.Angle;
	public HeightType HeightMode = HeightType.Relative;
	public float Angle = 45f;
	public float Height = 5.0f;
	public float Duration = 4.0f;

	private Parabola _Parabola;
	private float _StartTime = 0f;

	public struct Parabola
	{
		public float U;
		public float V;
		public float A;
		public float B;
		public float C;
		public Vector2 Start;
		public Vector2 End;
	};

	public enum DirectionType { Angle, Height }
	public enum HeightType { Absolute, Relative }

	float[,] Inverse3x3(float[,] matrix)
	{
		float[,] result = new float[3, 3];
		float det = matrix[0, 0] * (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1])
				  - matrix[0, 1] * (matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0])
				  + matrix[0, 2] * (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]);
		float invDet = 1.0f / det;
		result[0, 0] = (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1]) * invDet;
		result[0, 1] = -(matrix[0, 1] * matrix[2, 2] - matrix[0, 2] * matrix[2, 1]) * invDet;
		result[0, 2] = (matrix[0, 1] * matrix[1, 2] - matrix[0, 2] * matrix[1, 1]) * invDet;
		result[1, 0] = -(matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0]) * invDet;
		result[1, 1] = (matrix[0, 0] * matrix[2, 2] - matrix[0, 2] * matrix[2, 0]) * invDet;
		result[1, 2] = -(matrix[0, 0] * matrix[1, 2] - matrix[0, 2] * matrix[1, 0]) * invDet;
		result[2, 0] = (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]) * invDet;
		result[2, 1] = -(matrix[0, 0] * matrix[2, 1] - matrix[0, 1] * matrix[2, 0]) * invDet;
		result[2, 2] = (matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0]) * invDet;
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
		float[] result = Multiply3x3(inverse, new float[] { y1, y2, y3 });
		return new Vector3(result[0], result[1], result[2]);
	}

	Parabola ParabolaStart(Vector3 source, Vector3 destination, float angle, float height, DirectionType directionMode, HeightType heightMode)
	{
		Vector2 src = new Vector2(source.x, source.z);
		Vector2 dst = new Vector2(destination.x, destination.z);
		float u = 0.0f;
		float v = Vector2.Distance(dst, src);
		float x1 = u;
		float y1 = source.y;
		float x2 = Mathf.Lerp(u, v, 0.5f);
		if (directionMode == DirectionType.Angle) height = Mathf.Tan(angle * Mathf.Deg2Rad) * x2;
		if (heightMode == HeightType.Relative) height += Mathf.Lerp(source.y, destination.y, 0.5f);
		float y2 = height;
		float x3 = v;
		float y3 = destination.y;
		Vector3 abc = GetCoefficients(x1, y1, x2, y2, x3, y3);
		float a = abc.x; 
		float b = abc.y;
		float c = abc.z;
		Parabola parabola;
		parabola.U = u;
		parabola.V = v;
		parabola.A = a;
		parabola.B = b;
		parabola.C = c;
		parabola.Start = src;
		parabola.End = dst;
		return parabola;
	}

	Vector3 ParabolaUpdate(Parabola parabola, float currentTime, float duration)
	{
		float x = Mathf.Lerp(parabola.U, parabola.V, currentTime / duration);
		float y = parabola.A * x * x + parabola.B * x + parabola.C;
		Vector2 result = Vector2.Lerp(parabola.Start, parabola.End, x / parabola.V);
		Vector3 position = new Vector3(result.x, y, result.y);
		return position;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			_Parabola = ParabolaStart(Source.position, Destination.position, Angle, Height, DirectionMode, HeightMode);
			_StartTime = Time.time;
		}
		if (_StartTime > 0.0f)
		{
			float currentTime = Time.time - _StartTime;
			Bullet.position = ParabolaUpdate(_Parabola, currentTime, Duration);
		}
	}
}

/*
Theory:

To find the equation of a quadratic (parabola) with given 3 points, you can use the following steps:

Write the general equation for a quadratic function: y = ax^2 + bx + c.

Substitute the x and y values of the first point into the equation to get an equation with only a, b, and c as variables. Repeat this for the second and third points.

You will now have 3 equations with 3 unknowns (a, b, and c). Solve these equations simultaneously to find the values of a, b, and c.

Once you have found a, b, and c, substitute them into the general equation of a quadratic function to get the specific equation for the parabola.

Here's an example:

Suppose we have three points: (-2, 4), (1, 1), and (4, 4). We want to find the equation of the parabola that passes through these three points.

The general equation for a quadratic function is y = ax^2 + bx + c.

Substituting the first point (-2, 4) into the equation, we get: 4 = 4a - 2b + c

Substituting the second point (1, 1) into the equation, we get: 1 = a + b + c

Substituting the third point (4, 4) into the equation, we get: 4 = 16a + 4b + c

Now we have 3 equations with 3 unknowns: 4a - 2b + c = 4 a + b + c = 1 16a + 4b + c = 4

Solving these equations simultaneously, we get: a = 1 b = -2 c = 4

Finally, we substitute the values of a, b, and c into the general equation of a quadratic function to get the specific equation for the parabola: y = x^2 - 2x + 4

So the equation of the parabola that passes through the points (-2, 4), (1, 1), and (4, 4) is y = x^2 - 2x + 4.

*/
