using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml.Serialization;

public class Plant
{
	public string Title;
	public int[] Values;
}

public class ObjectComparer : MonoBehaviour
{
	[DllImport("msvcrt.dll", CallingConvention=CallingConvention.Cdecl)]
	static extern int memcmp(byte[] b1, byte[] b2, long count);

	byte[] ObjectToByteArray(object obj)
	{
		MemoryStream memoryStream = new MemoryStream();
		XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
		xmlSerializer.Serialize(memoryStream, obj);
		return memoryStream.ToArray();
	}

	bool IsEqual(object x, object y)
	{
		byte[] b1 = ObjectToByteArray(x);
		byte[] b2 = ObjectToByteArray(y);
		return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
	}

	void Start()
	{
		Plant apple = new Plant();
		apple.Title = "Apple";
		apple.Values = new int[5] {8, 16, 33, 47, 99};		

		Plant apple2 = new Plant();
		apple2.Title = "Apple";
		apple2.Values = new int[5] {8, 16, 33, 47, 99};	

		Plant pear = new Plant();
		pear.Title = "Pear";
		pear.Values = new int[5] {8, 16, 33, 47, 99};			

		Debug.Log(IsEqual(apple, apple2)); //true
		Debug.Log(IsEqual(apple, pear)); //false
	}
}