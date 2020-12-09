// Example how dynamically load and unload native DLL with Unity

/*
//For x64 Visual Studio command line:  cl.exe /LD Library.cpp

#include<algorithm>

extern "C" 
{
	__declspec(dllexport) unsigned char* SortBytes (unsigned char* bytes, int size)
	{
		std::sort(bytes, bytes + size);
		return bytes;
	}
}
*/

using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class Library : MonoBehaviour
{
	[DllImport("kernel32")]
	public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

	[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
	public static extern IntPtr LoadLibrary(string lpFileName);	

	[DllImport("kernel32", SetLastError = true)]
	public static extern bool FreeLibrary(IntPtr hModule);	

	delegate IntPtr SortBytes(IntPtr bytes, int size);
	
	IntPtr _Handle;

	void Init()
	{
		_Handle = LoadLibrary("D:/C++/Library.dll"); //set DLL absolute file path
		if (_Handle == IntPtr.Zero) Debug.LogError("Failed to load native library");
	}

	void Release()
	{
		FreeLibrary(_Handle);
	}

	T Invoke<T, T2>(IntPtr lib, params object[] args)
	{
		Delegate function = Marshal.GetDelegateForFunctionPointer(GetProcAddress(lib, typeof(T2).Name), typeof(T2));
		return (T)function.DynamicInvoke(args);
	}

	void LoadFunction()
	{
		byte[] bytes = { 65, 240, 245, 44, 158, 86, 180, 174, 207, 124 };
		int size = Marshal.SizeOf(bytes[0]) * bytes.Length;
		IntPtr buffer = Marshal.AllocHGlobal(size);
		Marshal.Copy(bytes, 0, buffer, bytes.Length);
		buffer = Invoke<IntPtr, SortBytes>(_Handle, buffer, bytes.Length);
		Marshal.Copy(buffer, bytes, 0, bytes.Length);
		for (int i = 0; i < bytes.Length; i++) Debug.Log(bytes[i]);
		Marshal.FreeHGlobal(buffer);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.I)) Init();
		if (Input.GetKeyDown(KeyCode.O)) LoadFunction();
		if (Input.GetKeyDown(KeyCode.P)) Release();	
	}
}