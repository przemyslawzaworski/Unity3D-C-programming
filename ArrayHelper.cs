using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public static class ArrayHelper
{
	/* Integer */
	
	public static void SaveIntArrayToFile(int[] x, string path)
	{		
		byte[] a = new byte[x.Length * 4];
		Buffer.BlockCopy(x, 0, a, 0, a.Length);
		File.WriteAllBytes(path,a);	
	}
	
	public static int[] LoadIntArrayFromFile(string path)
	{
		byte[] a = File.ReadAllBytes(path);
		int[] b = new int[a.Length / 4];
		Buffer.BlockCopy(a, 0, b, 0, a.Length);
		return b;
	}

	public static void SaveInt2ArrayToFile(int[,] x, string path) 
	{
		byte[] a = new byte[x.GetLength(0) * x.GetLength(1) * 4];
		Buffer.BlockCopy(x, 0, a, 0, a.Length);
		File.WriteAllBytes(path,a);	
	}
	
	public static int[,] LoadInt2ArrayFromFile(string path, int k) //k==size, for 5x5 k=5 etc...
	{
		byte[] a = File.ReadAllBytes(path);
		int[,] b = new int[a.Length / 4 / k, a.Length / 4 / k];
		Buffer.BlockCopy(a, 0, b, 0, a.Length);
		return b;
	}
	
	public static void SaveInt3ArrayToFile(int[,,] x, string path) 
	{
		byte[] a = new byte[x.GetLength(0) * x.GetLength(1) * x.GetLength(2)* 4];
		Buffer.BlockCopy(x, 0, a, 0, a.Length);
		File.WriteAllBytes(path,a);	
	}
	
	public static int[,,] LoadInt3ArrayFromFile(string path, int k) //k==size, for 5x5 k=5 etc...
	{
		byte[] a = File.ReadAllBytes(path);
		int[,,] b = new int[a.Length / 4 / k / k, a.Length / 4 / k / k, a.Length / 4 / k / k];
		Buffer.BlockCopy(a, 0, b, 0, a.Length);
		return b;
	}	

	/* Float */
	
	public static void SaveFloatArrayToFile(float[] x, string path)
	{		
		byte[] a = new byte[x.Length * 4];
		Buffer.BlockCopy(x, 0, a, 0, a.Length);
		File.WriteAllBytes(path,a);	
	}
	
	public static float[] LoadFloatArrayFromFile(string path)
	{
		byte[] a = File.ReadAllBytes(path);
		float[] b = new float[a.Length / 4];
		Buffer.BlockCopy(a, 0, b, 0, a.Length);
		return b;
	}
	
	public static void SaveFloat2ArrayToFile(float[,] x, string path) 
	{
		byte[] a = new byte[x.GetLength(0) * x.GetLength(1) * 4];
		Buffer.BlockCopy(x, 0, a, 0, a.Length);
		File.WriteAllBytes(path,a);	
	}
	
	public static float[,] LoadFloat2ArrayFromFile(string path, int k) //k==size, for 5x5 k=5 etc...
	{
		byte[] a = File.ReadAllBytes(path);
		float[,] b = new float[a.Length / 4 / k, a.Length / 4 / k];
		Buffer.BlockCopy(a, 0, b, 0, a.Length);
		return b;
	}

	public static void SaveFloat3ArrayToFile(float[,,] x, string path) 
	{
		byte[] a = new byte[x.GetLength(0) * x.GetLength(1) * x.GetLength(2)* 4];
		Buffer.BlockCopy(x, 0, a, 0, a.Length);
		File.WriteAllBytes(path,a);	
	}
	
	public static float[,,] LoadFloat3ArrayFromFile(string path, int x, int y, int z) 
	{
		byte[] a = File.ReadAllBytes(path);
		float[,,] b = new float[a.Length / 4 / x / z, a.Length / 4 / y / z, a.Length / 4 / x / y];
		Buffer.BlockCopy(a, 0, b, 0, a.Length);
		return b;
	}	
}