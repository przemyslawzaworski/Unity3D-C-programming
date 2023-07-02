using UnityEngine;

public class Statistics : MonoBehaviour
{
	double PearsonCorrelation(double[] x, double[] y) // calculate Pearson correlation coefficient
	{
		if (x.Length != y.Length)
			throw new System.ArgumentException("Input arrays must have the same length.");
		int n = x.Length;
		double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0, sumY2 = 0;
		for (int i = 0; i < n; i++)
		{
			sumX += x[i];
			sumY += y[i];
			sumXY += x[i] * y[i];
			sumX2 += x[i] * x[i];
			sumY2 += y[i] * y[i];
		}
		double meanX = sumX / n;
		double meanY = sumY / n;
		double numerator = sumXY - n * meanX * meanY;
		double denominator = System.Math.Sqrt((sumX2 - n * meanX * meanX) * (sumY2 - n * meanY * meanY));
		double correlation = numerator / denominator;
		return correlation;
	}

	double LinearRegression(double v, double[] x, double[] y)  // returns a predicted value assuming linear regression
	{
		if (x.Length != y.Length)
			throw new System.ArgumentException("Input arrays must have the same length.");
		int n = x.Length;
		double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
		for (int i = 0; i < n; i++)
		{
			sumX += x[i];
			sumY += y[i];
			sumXY += x[i] * y[i];
			sumX2 += x[i] * x[i];
		}
		double meanX = sumX / n;
		double meanY = sumY / n;
		double numerator = sumXY - n * meanX * meanY;
		double denominator = sumX2 - n * meanX * meanX;
		double slope = numerator / denominator;
		double intercept = meanY - slope * meanX;
		double prediction = intercept + slope * v;
		return prediction;
	}

	void Start()
	{
		double[] a = new double[] {-48.23, 13.87, 8.94, -57.21, 173.29, 151.65, 141.53, 96.76, -86.03, 58.2, 45.74};
		double[] b = new double[] {-86.04, 102.71, 99.06, -70.64, 14.0, -5.98, 53.78, 35.29, -30.45, -34.53, 123.26};
		UnityEngine.Debug.Log(PearsonCorrelation(a, b));
		UnityEngine.Debug.Log(LinearRegression(-51.337, a, b));
	}
}