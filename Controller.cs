using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class Controller : MonoBehaviour
{
	[DllImport("winmm.dll")]
	public static extern uint joyGetPosEx(uint uJoyID, ref JOYINFOEX pji);
	
	[StructLayout(LayoutKind.Sequential)]
	public struct JOYINFOEX
	{
		public uint dwSize;
		public uint dwFlags;
		public uint dwXpos;
		public uint dwYpos;
		public uint dwZpos;
		public uint dwRpos;
		public uint dwUpos;
		public uint dwVpos;
		public uint dwButtons;
		public uint dwButtonNumber;
		public uint dwPOV;
		public uint dwReserved1;
		public uint dwReserved2;
	}

	private JOYINFOEX joyinfo = new JOYINFOEX();
	private float _Deadzone = 0.1f;
	private float _TranslationSpeed = 0.5f;
	private float _RotationSpeed = 2.0f;
	private float _Yaw = 0.0f;
	private float _Pitch = 0.0f;

	float Remap (ulong x) // remap from (0..65535) to (-1,1) 
	{
		return (float) x / 65535.0f * 2.0f - 1.0f;
	}

	float InverseRemap (ulong x) // remap from (0..65535) to (1,-1)
	{
		return (float) x / 65535.0f * (-2.0f) + 1.0f;
	}

	void Init()
	{
		joyinfo.dwSize = (uint)Marshal.SizeOf(joyinfo);
		joyinfo.dwFlags = (uint)(0x00000001 | 0x00000002 | 0x00000004 | 0x00000008 | 0x00000010 | 0x00000020 | 0x00000040 | 0x00000080);
	}

	bool IsControllerDetected { get {return(joyGetPosEx(0, ref joyinfo) == 0);} }

	ulong GetInput { get {return IsControllerDetected ? joyinfo.dwButtons : 0;} }

	ulong GetNumberOfPressedButtons { get {return IsControllerDetected  ? joyinfo.dwButtonNumber : 0;} }

	float GetLeftStickHorizontal { get {return IsControllerDetected  ? Remap(joyinfo.dwXpos) : 0.0f;} }

	float GetLeftStickVertical { get {return IsControllerDetected  ? InverseRemap(joyinfo.dwYpos) : 0.0f;} }

	float GetRightStickHorizontal { get {return IsControllerDetected  ? Remap(joyinfo.dwZpos) : 0.0f;} }

	float GetRightStickVertical { get {return IsControllerDetected  ? InverseRemap(joyinfo.dwRpos) : 0.0f;} }

	void PrintState ()
	{
		switch (GetInput)
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
		float mx = GetLeftStickHorizontal;
		float mz = GetLeftStickVertical;
		if ((Mathf.Abs(mx) > _Deadzone) || (Mathf.Abs(mz) > _Deadzone))
		{
			transform.Translate(new Vector3(mx, 0.0f, mz) * _TranslationSpeed);
		}
	}

	void Rotation ()
	{
		float rx = GetRightStickHorizontal;
		float ry = GetRightStickVertical;
		if ((Mathf.Abs(rx) > _Deadzone) || (Mathf.Abs(ry) > _Deadzone)) 
		{
			_Yaw   += _RotationSpeed * rx;
			_Pitch -= _RotationSpeed * ry;
			transform.eulerAngles = new Vector3(_Pitch, _Yaw, 0.0f);
		}
	}

	void Start()
	{
		Init();
	}

	void Update()
	{
		PrintState ();
		Translation ();
		Rotation ();
	}

	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 100, 20), IsControllerDetected.ToString());
	}
}