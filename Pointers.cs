using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class Pointers : MonoBehaviour
{
	public struct Structure
	{
		public int Number;
	}

	void PrintFloat()
	{
		unsafe
		{
			IntPtr intPtr = Marshal.AllocHGlobal(sizeof(float));
			float* number = (float*)intPtr;
			*number = 5.0f;
			Debug.Log("Value: " + *number);
			Marshal.FreeHGlobal(intPtr);
		}
	}

	void PrintInteger()
	{
		unsafe
		{
			int number = 12;
			int* pointer = &number;
			Debug.Log("Address of number: " + (IntPtr)pointer);
			Debug.Log("Value at the address: " + *pointer);
			*pointer = 100;
			Debug.Log("Modified value at the address: " + *pointer);
			Debug.Log("Value of number after modification: " + number);
		}
	}

	void PrintStruct()
	{
		unsafe
		{
			Structure structure;
			structure.Number = 56;
			Structure* pointer = &structure;
			Debug.Log("Address of structure: " + (IntPtr)pointer);
			Debug.Log("Number: " + pointer->Number);
			pointer->Number = 200;
			Debug.Log("Modified Number: " + pointer->Number);
			Debug.Log("structure.Number after modification: " + structure.Number);
		}
	}

	void Start()
	{
		PrintFloat();
		Debug.Log("-----------------------------");
		PrintInteger();
		Debug.Log("-----------------------------");
		PrintStruct();
	}
}