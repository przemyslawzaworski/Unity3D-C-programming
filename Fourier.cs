using UnityEngine;

public class Fourier : MonoBehaviour
{
	// Discrete Fourier Transform
	// https://en.wikipedia.org/wiki/Discrete_Fourier_transform
	void CalculateDFT(double[,] input, double[,] output)
	{
		int N = input.GetLength(0);
		for (int n = 0; n < N; n++) 
		{
			double cr = 0; // complex real 
			double ci = 0; // complex imaginary
			for (int k = 0; k < N; k++) 
			{
				double angle = 2 * System.Math.PI / N * k * n;
				cr +=  input[k, 0] * System.Math.Cos(angle) + input[k, 1] * System.Math.Sin(angle);
				ci += -input[k, 0] * System.Math.Sin(angle) + input[k, 1] * System.Math.Cos(angle);
			}
			output[n, 0] = cr;
			output[n, 1] = ci;
		}
	}

	// Inverse Discrete Fourier Transform
	void CalculateIDFT(double[,] input, double[,] output)
	{
		int N = input.GetLength(0);
		for (int n = 0; n < N; n++) 
		{
			double cr = 0; // complex real 
			double ci = 0; // complex imaginary
			for (int k = 0; k < N; k++) 
			{
				double angle = 2 * System.Math.PI / N * k * n;
				cr += input[k, 0] * System.Math.Cos(angle) - input[k, 1] * System.Math.Sin(angle);
				ci += input[k, 0] * System.Math.Sin(angle) + input[k, 1] * System.Math.Cos(angle);
			}
			output[n, 0] = cr / (double) N;
			output[n, 1] = ci / (double) N;
		}
	}	

	void Start ()
	{
		double[,] a = new double[,] {{34,0}, {27,0}, {25,0}, {2,0}, {47,0}, {32,0}, {17,0}, {35,0}, {30,0}, {33,0}};
		double[,] b = new double[10, 2];
		double[,] c = new double[10, 2];
		CalculateDFT(a, b);
		for (int i = 0; i < b.GetLength(0); i++)
		{
			Debug.Log(b[i,0].ToString("N3") + " " + b[i,1].ToString("+ 0.###;- 0.###") + "j");
		}
		Debug.Log("--------------------------------------------------------");
		CalculateIDFT(b, c);
		for (int i = 0; i < c.GetLength(0); i++)
		{
			Debug.Log(c[i,0].ToString("N3") + " " + c[i,1].ToString("+ 0.###;- 0.###") + "j");
		}
	}
}