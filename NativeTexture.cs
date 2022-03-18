using System.Runtime.InteropServices;
using UnityEngine;
using System;
using System.IO;

// Compile NativeTexture.cpp file and then copy DLL into Assets/Plugins.
// Before start, create example.tga as uncompressed 32-bit TGA (for example in Gimp), and insert into Streaming Assest.
public class NativeTexture : MonoBehaviour
{
	[DllImport("NativeTexture")]
	static extern IntPtr TextureApply (IntPtr bytes, uint width, uint height);

	[DllImport("NativeTexture")]
	static extern void TextureRelease ( );

	private bool _IsLoaded = false;

	bool LoadTexture(string path)
	{
		Stream stream = File.OpenRead(path);
		BinaryReader reader = new BinaryReader(stream);
		reader.BaseStream.Seek(12, SeekOrigin.Begin);
		short width = reader.ReadInt16();
		short height = reader.ReadInt16();
		reader.BaseStream.Seek(2, SeekOrigin.Current);
		byte[] bytes = reader.ReadBytes(width * height * 4);
		reader.Close();
		stream.Close();
		IntPtr handle = Marshal.AllocHGlobal((int)width * (int)height * 4);
		Marshal.Copy(bytes, 0, handle, bytes.Length);
		IntPtr srv = TextureApply(handle, (uint)width, (uint)height);
		Texture2D texture = Texture2D.CreateExternalTexture((int)width, (int)height, TextureFormat.BGRA32, false, false, srv);
		this.GetComponent<Renderer>().material.mainTexture = texture;
		Marshal.FreeHGlobal(handle);
		return true;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.O) && !_IsLoaded)
		{
			_IsLoaded = LoadTexture(Path.Combine(Application.streamingAssetsPath, "example.tga"));
		}
		if (Input.GetKeyDown(KeyCode.R) && _IsLoaded)
		{
			TextureRelease();
			GetComponent<Renderer>().material.mainTexture = Texture2D.whiteTexture;
			_IsLoaded = false;
		}
	}
}