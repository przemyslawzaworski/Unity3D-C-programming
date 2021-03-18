// Example how to read/write objects from/to file, without serialization/deserialization.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.IO;

[StructLayout(LayoutKind.Sequential, Pack = 1)]  //read more: http://www.pzielinski.com/?p=1337
public class Tree
{
	[MarshalAs(UnmanagedType.LPStr, SizeConst = 1)]  //allocate memory for one element of type string
	public string Title;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]  //allocate memory for five elements of type integer
	public int[] Values;
}

public class ObjectManager : MonoBehaviour
{
	public enum State {Write, Read}	
	public State Mode = State.Write;

	void SaveObjectToFile (System.Object structure, string path)
	{
		int size = Marshal.SizeOf(structure);
		byte[] bytes = new byte[size];
		IntPtr ptr = Marshal.AllocHGlobal(size);
		Marshal.StructureToPtr(structure, ptr, false);
		Marshal.Copy(ptr, bytes, 0, size);
		Marshal.FreeHGlobal(ptr);
		File.WriteAllBytes (path, bytes);
	}

	T LoadObjectFromFile<T> (string path)
	{
		byte[] bytes = File.ReadAllBytes (path);
		int size = bytes.Length;
		IntPtr ptr = Marshal.AllocHGlobal(size);
		Marshal.Copy(bytes, 0, ptr, size);
		T structure = (T)Marshal.PtrToStructure(ptr, typeof(T));
		Marshal.FreeHGlobal(ptr);
		return structure;
	}

	void Start()
	{
		if (Mode == State.Write)
		{
			Tree tree = new Tree();
			tree.Title = "Apple";
			tree.Values = new int[5] {8, 16, 33, 47, 99};
			SaveObjectToFile (tree, Path.Combine(Application.streamingAssetsPath, "test.bin"));
		}

		if (Mode == State.Read)
		{
			Tree tree = LoadObjectFromFile<Tree> (Path.Combine(Application.streamingAssetsPath, "test.bin"));
			Debug.Log(tree.Title);
			for (int i = 0; i < tree.Values.Length; i++) Debug.Log(tree.Values[i]);
		}
	}
}
