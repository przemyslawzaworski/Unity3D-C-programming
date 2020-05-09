using System;
using System.IO;
using UnityEngine;
 
public static class LoadTGA
{
	// Loads 32-bit (RGBA) uncompressed TGA. Actually, due to TARGA file structure, BGRA32 is good option...
	// Disabled mipmaps. Disabled read/write option, to release texture memory copy.
	public static Texture2D Load(string fileName)
	{
		try
		{
			BinaryReader reader = new BinaryReader(File.OpenRead(fileName));
			reader.BaseStream.Seek(12, SeekOrigin.Begin);     
			short width = reader.ReadInt16();
			short height = reader.ReadInt16();
			reader.BaseStream.Seek(2, SeekOrigin.Current);
			byte[] source = reader.ReadBytes(width * height * 4);
			reader.Close();
			Texture2D texture = new Texture2D(width, height, TextureFormat.BGRA32, false);
			texture.LoadRawTextureData(source);
			texture.name = Path.GetFileName(fileName);
			texture.Apply(false, true);
			return texture;
		}
		catch (Exception)
		{
			return Texture2D.blackTexture;
		}
	}
}