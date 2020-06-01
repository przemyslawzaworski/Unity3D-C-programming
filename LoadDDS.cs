using UnityEngine;
using System.IO;
using System;

public static class LoadDDS
{
	public static Texture2D Load(string path)
	{
		try
		{
			BinaryReader reader = new BinaryReader(File.OpenRead(path));
			long length = new FileInfo(path).Length;
			byte[] header = reader.ReadBytes(128);
			int height = header[13] * 256 + header[12];
			int width = header[17] * 256 + header[16];
			bool mipmaps = header[28] > 0;
			TextureFormat textureFormat = header[87] == 49 ? TextureFormat.DXT1 : TextureFormat.DXT5;
			byte[] source = reader.ReadBytes(Convert.ToInt32(length) - 128);
			reader.Close();
			Texture2D texture = new Texture2D(width, height, textureFormat, mipmaps);
			texture.LoadRawTextureData(source);
			texture.name = Path.GetFileName(path);
			texture.Apply(false, true);
			return texture;
		}
		catch (Exception)
		{
			return Texture2D.blackTexture;
		}
	}
}