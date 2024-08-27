using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class ProceduralTextureUnsafe : MonoBehaviour
{
	public GameObject Quad;
	public int Width = 128;
	public int Height = 128;
	[Range(1, 16)] public int Quality = 12;

	private Texture2D _Texture;
	private int _Size;
	private IntPtr _Data;
	private unsafe int* _Pixels;

	unsafe void Init()
	{
		_Size = Width * Height * sizeof(int);
		_Data = Marshal.AllocHGlobal(_Size);
		_Texture = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
		_Pixels = (int*)_Data.ToPointer();
		Quad.GetComponent<Renderer>().material.mainTexture = _Texture;
	}

	float SandWaves(float x, float y, float h, float time, int octaves, float amplitude, float scale)
	{
		float radius = 8.0f;
		float frequency = 0.03f;
		float speed = -0.05f;
		for (int i = 0; i < octaves; i++)
		{
			float s = Mathf.Sin(time * 0.021f) * 0.1f + 0.41f * Mathf.Sin(i * 0.71f + time * 0.02f);
			float angle = 1.8f + s * amplitude;
			float dx = Mathf.Cos(angle);
			float dy = Mathf.Sin(angle);
			float t = -1.0f * ((x - scale) * dx + (y - scale) * dy);
			float sineWave = Mathf.Sin(time * speed + t * frequency) * radius;
			float cosineWave = Mathf.Cos(time * speed + t * frequency) * radius;
			x -= cosineWave * dx * 2.0f;
			y -= cosineWave * dy * 2.0f;
			h -= sineWave;
			radius *= 0.72f;
			frequency *= 1.27f;
			speed *= 1.21f;
		}
		h = h + 22.0f;
		h *= 0.018f;
		return h;
	}

	unsafe void Generate()
	{
		float scale = 512.0f;
		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				int index = y * Width + x;
				float uvX = x / (float)Width * scale;
				float uvY = y / (float)Height * scale;
				float h = SandWaves(uvX, uvY, 0.0f, Time.time * 20.0f, Quality, 1.0f, scale);
				byte r = (byte)Mathf.Min((0.3f + h * 1.2f) * 255, 255);
				byte g = (byte)Mathf.Min((0.2f + h * 0.9f) * 255, 255);
				byte b = (byte)Mathf.Min((0.1f + h * 0.6f) * 255, 255);
				byte a = 255;
				_Pixels[index] = (a << 24) | (b << 16) | (g << 8) | r;
			}
		}
		_Texture.LoadRawTextureData(_Data, _Size);
		_Texture.Apply();
	}

	void Start()
	{
		Init();
	}

	void Update()
	{
		Generate();
	}

	void OnDestroy()
	{
		if (_Data != IntPtr.Zero) Marshal.FreeHGlobal(_Data);
		if (_Texture != null) Destroy(_Texture);
	}
}