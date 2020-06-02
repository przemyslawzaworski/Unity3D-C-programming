using UnityEngine;
using System;
using System.IO;

public class LoadTextureAsyncFromDDS : MonoBehaviour
{
	async void Load (Material material, string property, string filepath)
	{
		FileStream stream = File.Open(filepath, FileMode.Open);
		long length = stream.Length;
		byte[] header = new byte[128];
		await stream.ReadAsync(header, 0, 128);
		int height = header[13] * 256 + header[12];
		int width = header[17] * 256 + header[16];
		bool mipmaps = header[28] > 0;
		TextureFormat textureFormat = header[87] == 49 ? TextureFormat.DXT1 : TextureFormat.DXT5;
		byte[] source = new byte[Convert.ToInt32(length) - 128];
		await stream.ReadAsync(source, 0, Convert.ToInt32(length) - 128);
		stream.Close();
		Texture2D texture = new Texture2D(width, height, textureFormat, mipmaps);
		texture.LoadRawTextureData(source);
		texture.name = Path.GetFileName(filepath);
		texture.Apply(false, true);
		material.SetTexture(property, texture);
	}

	void Delete (Material material, string property)
	{
		Destroy(material.GetTexture(property));
		material.SetTexture(property, null);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.L))
			Load(GetComponent<Renderer>().material, "_MainTex", System.IO.Path.Combine(Application.streamingAssetsPath, "plasma.dds"));
		if (Input.GetKeyDown(KeyCode.Space))
			Delete(GetComponent<Renderer>().material, "_MainTex");
	}
}