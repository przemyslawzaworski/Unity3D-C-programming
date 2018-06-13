using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class save_float_array_to_file : MonoBehaviour 
{
	float[] p = new float[] { 744.385f,-318.609f, 33.538f, 217.61f };
	
	void SaveFloatArrayToFile(float[] x, string path)
	{		
		byte[] a = new byte[x.Length * 4];
		Buffer.BlockCopy(x, 0, a, 0, a.Length);
		File.WriteAllBytes(path,a);	
	}
	
	float[] LoadFloatArrayFromFile(string path)
	{
		byte[] a = File.ReadAllBytes(path);
		float[] b = new float[a.Length / 4];
		Buffer.BlockCopy(a, 0, b, 0, a.Length);
		return b;
	}
	
	void Start () 
	{
		SaveFloatArrayToFile(p,"C:\\array.dat");
		float[] n = LoadFloatArrayFromFile("C:\\array.dat");	
		for (int i=0;i<n.Length;i++) Debug.Log(n[i]);	
	}

}
