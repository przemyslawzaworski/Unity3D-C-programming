/* C++ DLL code:

#include <string>
#define DllExport __declspec(dllexport)

extern "C" 
{
	DllExport char* CombineStrings(char* s1,char* s2);
	
	char* faststrcat( char* dest, char* src )
	{
		while (*dest) dest++;
		while (*dest++ = *src++);
		return --dest;
	}
	
	char* CombineStrings( char* s1, char* s2 )
	{
		char* s3 = (char*) malloc(1+strlen(s1)+strlen(s2));
		strcpy(s3, s1);
		faststrcat(s3, s2);
		return s3;
	}
}

*/

using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class NativeCombineString : MonoBehaviour 
{
	public string A;
	public string B;
	[DllImport("CombineString", EntryPoint = "CombineStrings")]
	static extern IntPtr CombineStrings([MarshalAs(UnmanagedType.LPStr)] string x, [MarshalAs(UnmanagedType.LPStr)] string y);
	IntPtr handle;
	String caption;

	void Update()
	{
		handle = CombineStrings (A, B);
		caption = Marshal.PtrToStringAnsi (handle);
	}

	void OnGUI() 
	{
		GUILayout.Label (caption);
	}
}