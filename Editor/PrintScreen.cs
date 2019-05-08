using UnityEngine;
using UnityEditor;
using System;
using System.Runtime.InteropServices;

public class PrintScreen : EditorWindow
{
	[DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
	
	[MenuItem("Assets/PrintScreen")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow ( typeof(PrintScreen));
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Print" ) ) PrintNow();
	} 

	void PrintNow()
	{
		string temp = Application.temporaryCachePath + "//test.png";
		ScreenCapture.CaptureScreenshot(temp);
		ShellExecute(IntPtr.Zero, "print", temp, null, null, 1);
    }
}
