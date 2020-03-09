/*
	PS4 controller input, assignment for return values from GetInput() :
    Square   = 1
    X        = 2
    Circle   = 4
    Triangle = 8
    L1       = 16
    R1       = 32
    L2       = 64
    R2       = 128
    Share	 = 256
    Options  = 512
    L3       = 1024
    R3       = 2048
    PS       = 4096
    PadPress = 8192
*/

/*
DLL code, save as controller.c and compile: cl.exe /LD controller.c user32.lib gdi32.lib winmm.lib
then copy DLL into Assets/Plugins and restart editor
Only works if one gamepad is connected.
*/

/* 
#include <windows.h>
#include <mmsystem.h>

JOYINFOEX joyinfo; 

float Remap (unsigned long x) // remap from (0..65535) to (-1,1) 
{
	return (float) x / 65535.0f * 2.0f - 1.0f;
}

float InverseRemap (unsigned long x) // remap from (0..65535) to (1,-1)
{
	return (float) x / 65535.0f * (-2.0f) + 1.0f;
}

int IsControllerDetected ()
{
	return (joyGetPosEx(0, &joyinfo) == 0);
}

__declspec(dllexport) void Init()
{
	joyinfo.dwSize = sizeof(joyinfo);
	joyinfo.dwFlags = JOY_RETURNALL;
}

__declspec(dllexport) unsigned long GetInput()
{
	return IsControllerDetected () ? joyinfo.dwButtons : 0;
}

__declspec(dllexport) unsigned long GetNumberOfPressedButtons()
{
	return IsControllerDetected () ? joyinfo.dwButtonNumber : 0;
}

__declspec(dllexport) float GetLeftStickHorizontal()
{
	return IsControllerDetected () ? Remap(joyinfo.dwXpos) : 0.0f;
}

__declspec(dllexport) float GetLeftStickVertical()
{
	return IsControllerDetected () ? InverseRemap(joyinfo.dwYpos) : 0.0f;
}

__declspec(dllexport) float GetRightStickHorizontal()
{
	return IsControllerDetected () ? Remap(joyinfo.dwZpos) : 0.0f;
}

__declspec(dllexport) float GetRightStickVertical()
{
	return IsControllerDetected () ? InverseRemap(joyinfo.dwRpos) : 0.0f;
}
*/

using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class Controller : MonoBehaviour 
{
	[DllImport("controller", EntryPoint = "Init")]
	static extern void Init ();
	[DllImport("controller", EntryPoint = "GetInput")]
	static extern uint GetInput ();
	[DllImport("controller", EntryPoint = "GetNumberOfPressedButtons")]
	static extern uint GetNumberOfPressedButtons ();	
	[DllImport("controller", EntryPoint = "GetLeftStickHorizontal")]
	static extern float GetLeftStickHorizontal ();
	[DllImport("controller", EntryPoint = "GetLeftStickVertical")]
	static extern float GetLeftStickVertical ();
	[DllImport("controller", EntryPoint = "GetRightStickHorizontal")]
	static extern float GetRightStickHorizontal ();
	[DllImport("controller", EntryPoint = "GetRightStickVertical")]
	static extern float GetRightStickVertical ();

	private float _Deadzone = 0.05f;	
	private float _TranslationSpeed = 0.5f;		
	private float _RotationSpeed = 2.0f;
	private float _Yaw = 0.0f;
	private float _Pitch = 0.0f;	
	
	void Start()
	{
		Init();
	}

	void PrintState ()
	{
		switch (GetInput ())
		{
			case 1:		Debug.Log("Pressed Square !"); break;
			case 2:		Debug.Log("Pressed X !"); break;
			case 4:		Debug.Log("Pressed Circle !"); break;
			case 8:		Debug.Log("Pressed Triangle !"); break;
			case 16:	Debug.Log("Pressed L1 !"); break;
			case 32:	Debug.Log("Pressed R1 !"); break;
			case 64:	Debug.Log("Pressed L2 !"); break;
			case 128:	Debug.Log("Pressed R2 !"); break;
			case 256:	Debug.Log("Pressed Share !"); break;
			case 512:	Debug.Log("Pressed Options !"); break;
			case 1024:	Debug.Log("Pressed L3 !"); break;
			case 2048:	Debug.Log("Pressed R3 !"); break;
			case 4096:	Debug.Log("Pressed PS !"); break;
			case 8192:	Debug.Log("Pressed PadPress !"); break;
		}
	}
	
	void Translation ()
	{
		float mx = GetLeftStickHorizontal ();
		float mz = GetLeftStickVertical ();
		if ((Mathf.Abs(mx) > _Deadzone) || (Mathf.Abs(mz) > _Deadzone))
		{
			transform.Translate(new Vector3(mx, 0.0f, mz) * _TranslationSpeed);
		}
	}
	
	void Rotation ()
	{
		float rx = GetRightStickHorizontal ();
		float ry = GetRightStickVertical ();
		if ((Mathf.Abs(rx) > _Deadzone) || (Mathf.Abs(ry) > _Deadzone)) 
		{
			_Yaw   += _RotationSpeed * rx;
			_Pitch -= _RotationSpeed * ry;
			transform.eulerAngles = new Vector3(_Pitch, _Yaw, 0.0f);
		}
	}
	
	void Update()
	{
		PrintState ();
		Translation ();
		Rotation ();
	}
}