using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
 
public class cdrom : MonoBehaviour 
{
	[DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)] 
	private static extern long mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);
	
	void Start () 
	{
		mciSendString ("Set cdaudio door open wait",null,0,0);
		mciSendString ("Set cdaudio door closed wait",null,0,0);
	}
}
