// https://unsaferguard.org/un-saferguard/kingery-bulmash
// Kingery-Bulmash equation calculates the blast-wave parameters of a hemispherical free field air-blast (explosive type: TNT)

using System;
using System.Collections.Generic;
using UnityEngine;

public class BlastWave : MonoBehaviour
{
	public double ChargeWeight = 100.0f; // kilograms
	public double Range = 30.0f; // meters

	public class BlastParameters
	{
		public double? IncidentPressure = null; // kPa
		public double? ReflectedPressure = null; // kPa
		public double? TimeOfArrival = null; // miliseconds
		public double? ShockFrontVelocity = null; // m/s
		public double? IncidentImpulse = null; // kPa-ms
		public double? ReflectedImpulse = null; // kPa-ms
		public double? PositivePhaseDuration = null; // ms
	};

	private double Evaluation(double distance, params double[] coefficients) 
	{
		double logarithm = Math.Log(distance);
		double total = 0.0;
		for (int i = 0; i < coefficients.Length; i++)
		{
			total += (i == 0) ? coefficients[i] : coefficients[i] * Math.Pow(logarithm, i);
		}
		return Math.Exp(total);
	}

	private BlastParameters KingeryBulmash()
	{
		double root = Math.Pow(ChargeWeight, 1.0 / 3.0);
		double distance = Range / root; // Hopkinson-Cranz scaled distance
		BlastParameters blastParameters = new BlastParameters();
		if (0.2 <= distance && distance <= 2.9)
			blastParameters.IncidentPressure = Evaluation(distance, 7.2106, -2.1069, -0.3229, 0.1117, 0.0685, 0, 0);
		else if (2.9 < distance && distance <= 23.8)
			blastParameters.IncidentPressure = Evaluation(distance, 7.5938, -3.0523, 0.40977, 0.0261, -0.01267, 0, 0);
		else if (23.8 < distance && distance <= 198.5)
			blastParameters.IncidentPressure = Evaluation(distance, 6.0536, -1.4066, 0, 0, 0, 0, 0);
		if (0.06 <= distance && distance <= 2.00)
			blastParameters.ReflectedPressure = Evaluation(distance, 9.006, -2.6893, -0.6295, 0.1011, 0.29255, 0.13505, 0.019736);
		else if (2.00 < distance && distance <= 40)
			blastParameters.ReflectedPressure = Evaluation(distance, 8.8396, -1.733, -2.64, 2.293, -0.8232, 0.14247, -0.0099); 
		if (0.06 <= distance && distance <= 1.50)
			blastParameters.TimeOfArrival = Evaluation(distance, -0.7604, 1.8058, 0.1257, -0.0437, -0.0310, -0.00669, 0) * root;
		else if (1.50 < distance && distance <= 40)
			blastParameters.TimeOfArrival = Evaluation(distance, -0.7137, 1.5732, 0.5561, -0.4213, 0.1054, -0.00929, 0) * root; 
		if (0.06 <= distance && distance <= 1.50)
			blastParameters.ShockFrontVelocity = Evaluation(distance, 0.1794, -0.956, -0.0866, 0.109, 0.0699, 0.01218, 0) * 1000.0;
		else if (1.50 < distance && distance <= 40)
			blastParameters.ShockFrontVelocity = Evaluation(distance, 0.2597, -1.326, 0.3767, 0.0396, -0.0351, 0.00432, 0) * 1000.0;
		if (0.2 <= distance && distance <= 0.96)
			blastParameters.IncidentImpulse = Evaluation(distance, 5.522, 1.117, 0.6, -0.292, -0.087, 0, 0) * root;
		else if (0.96 < distance && distance <= 2.38)
			blastParameters.IncidentImpulse = Evaluation(distance, 5.465, -0.308, -1.464, 1.362, -0.432, 0, 0) * root;
		else if (2.38 < distance && distance <= 33.7)
			blastParameters.IncidentImpulse = Evaluation(distance, 5.2749, -0.4677, -0.2499, 0.0588, -0.00554, 0, 0) * root;
		else if (33.7 < distance && distance <= 158.7)
			blastParameters.IncidentImpulse = Evaluation(distance, 5.9825, -1.062, 0, 0, 0, 0, 0) * root; 
		if (0.06 <= distance && distance <= 40)
			blastParameters.ReflectedImpulse = Evaluation(distance, 6.7853, -1.3466, 0.101, -0.01123, 0, 0, 0) * root; 
		if (0.2 <= distance && distance <= 1.02)
			blastParameters.PositivePhaseDuration = Evaluation(distance, 0.5426, 3.2299, -1.5931, -5.9667, -4.0815, -0.9149, 0) * root;
		else if (1.02 < distance && distance <= 2.8)
			blastParameters.PositivePhaseDuration = Evaluation(distance, 0.5440, 2.7082, -9.7354, 14.3425, -9.7791, 2.8535, 0) * root;
		else if (2.8 < distance && distance <= 40)
			blastParameters.PositivePhaseDuration = Evaluation(distance, -2.4608, 7.1639, -5.6215, 2.2711, -0.44994, 0.03486, 0) * root;
		return blastParameters;
	}

	private void Start()
	{
		BlastParameters blastParameters = KingeryBulmash();
		Debug.Log("IncidentPressure: " + blastParameters.IncidentPressure.Value.ToString("G6")); 
		Debug.Log("ReflectedPressure: " + blastParameters.ReflectedPressure.Value.ToString("G6"));
		Debug.Log("TimeOfArrival: " + blastParameters.TimeOfArrival.Value.ToString("G6")); 
		Debug.Log("ShockFrontVelocity: " + blastParameters.ShockFrontVelocity.Value.ToString("G6"));
		Debug.Log("IncidentImpulse: " + blastParameters.IncidentImpulse.Value.ToString("G6"));
		Debug.Log("ReflectedImpulse: " + blastParameters.ReflectedImpulse.Value.ToString("G6")); 
		Debug.Log("PositivePhaseDuration: " + blastParameters.PositivePhaseDuration.Value.ToString("G6"));
	}
}