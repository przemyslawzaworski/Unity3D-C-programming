using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;

public class Cryptography : MonoBehaviour
{
	byte[] Encode(byte[] bytes, byte[] key, byte[] vector)
	{
		Aes aes = Aes.Create();
		ICryptoTransform encryptor = aes.CreateEncryptor(key, vector);
		MemoryStream memoryStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
		cryptoStream.Write(bytes, 0, bytes.Length);
		cryptoStream.Close();
		return memoryStream.ToArray();
	}

	byte[] Decode(byte[] bytes, byte[] key, byte[] vector)
	{
		Aes aes = Aes.Create();
		ICryptoTransform decryptor = aes.CreateDecryptor(key, vector);
		MemoryStream memoryStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);
		cryptoStream.Write(bytes, 0, bytes.Length);
		cryptoStream.Close();
		return memoryStream.ToArray();
	}

	void Start()
	{
		byte[] bytes = System.Text.Encoding.UTF8.GetBytes("This is a secret message!");
		byte[] key = new byte[16] { 25, 197, 154, 211, 240, 223, 182, 197, 205, 16, 147, 190, 55, 126, 152, 47 }; // The secret key to use for the symmetric algorithm.
		byte[] iv = new byte[16] { 101, 56, 27, 187, 183, 147, 214, 82, 84, 133, 193, 167, 118, 147, 253, 18 }; // The initialization vector to use for the symmetric algorithm.
		byte[] encrypted = Encode(bytes, key, iv);
		//File.WriteAllBytes(Path.Combine(Application.streamingAssetsPath, "config.dat"), encrypted);
		byte[] decrypted = Decode(encrypted, key, iv);
		//decrypted = Decode(File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, "config.dat")), key, iv);
		Debug.Log (System.Text.Encoding.UTF8.GetString(decrypted));
	}
}