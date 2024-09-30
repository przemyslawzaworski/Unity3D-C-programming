using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AVX : MonoBehaviour
{
	[DllImport("avx")]
	private static extern IntPtr GenerateTexture(int width, int height);

	[DllImport("avx")]
	private static extern void FreeMemory(IntPtr ptr);

	Material _Material;
	Texture2D _Texture;
	int Width {get; set;} = 1024;
	int Height {get; set;} = 1024;

	void Start()
	{
		_Material = GetComponent<MeshRenderer>().sharedMaterial;
		_Texture = new Texture2D(Width, Height, TextureFormat.RGBA32, false, false);
		_Material.mainTexture = _Texture;
	}

	void Update()
	{
		IntPtr pixelDataPtr = GenerateTexture(Width, Height);
		_Texture.LoadRawTextureData(pixelDataPtr, Width * Height * 4);
		_Texture.Apply();
		FreeMemory(pixelDataPtr);
	}

	void OnDestroy()
	{
		Destroy(_Texture);
	}
}

/*
// Compile DLL and put in Assets/Plugins
// cl /LD /O2 /arch:AVX avx.c
#include <immintrin.h>
#include <stdlib.h>

typedef unsigned char byte;

int RGBAToInt(byte r, byte g, byte b, byte a)
{
	return (a << 24) | (b << 16) | (g << 8) | r;
}

__declspec(dllexport) byte* GenerateTexture(int width, int height)
{
	byte* bytes = (byte*)malloc(width * height * 4);
	byte b = (byte)0;
	byte a = (byte)255;
	float w = (float)width;
	float h = (float)height;
	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < width; x+=8)
		{
			int p0 = RGBAToInt((byte)((x + 0) / w * 255), (byte)(y / h * 255), b, a);
			int p1 = RGBAToInt((byte)((x + 1) / w * 255), (byte)(y / h * 255), b, a);
			int p2 = RGBAToInt((byte)((x + 2) / w * 255), (byte)(y / h * 255), b, a);
			int p3 = RGBAToInt((byte)((x + 3) / w * 255), (byte)(y / h * 255), b, a);
			int p4 = RGBAToInt((byte)((x + 4) / w * 255), (byte)(y / h * 255), b, a);
			int p5 = RGBAToInt((byte)((x + 5) / w * 255), (byte)(y / h * 255), b, a);
			int p6 = RGBAToInt((byte)((x + 6) / w * 255), (byte)(y / h * 255), b, a);
			int p7 = RGBAToInt((byte)((x + 7) / w * 255), (byte)(y / h * 255), b, a);
			int i = x + width * y;
			__m256i pixels = _mm256_set_epi32(p7, p6, p5, p4, p3, p2, p1, p0);
			_mm256_storeu_si256((__m256i*)&bytes[i * 4], pixels);
		}
	}
	return bytes;
}

__declspec(dllexport) void FreeMemory(byte* pointer)
{
	free(pointer);
}

*/
