// Dump systeminfo to output.log (build) or console
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class error : MonoBehaviour
{
	void OnEnable()
	{
		Application.logMessageReceived += LogCallback;
	}

	void LogCallback(string condition, string stackTrace, LogType type)
	{
		string result = "";
		foreach(var property in typeof(SystemInfo).GetProperties()) 
		{
			if (property.CanRead) result = result + property.Name + " = " + property.GetValue(null).ToString() + " \n";
		}
		Debug.Log(result);
	}

	void OnDisable()
	{
		Application.logMessageReceived -= LogCallback;
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			float num = System.Single.Parse("abc");   //simulates error
		}
	}
}