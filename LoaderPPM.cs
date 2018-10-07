/*

#include <stdio.h>
#include <stdlib.h>

extern "C" 
{
	__declspec(dllexport) unsigned char* LoadPPM(char* filename)
	{
		int width, height;
		char buffer[128]; 
		FILE* file = fopen(filename, "rb");
		fgets(buffer, sizeof(buffer), file);
		do fgets(buffer, sizeof (buffer), file); while (buffer[0] == '#');
		sscanf (buffer, "%d %d", &width, &height);
		do fgets (buffer, sizeof (buffer), file); while (buffer[0] == '#');
		int size = width * height * 4 * sizeof(unsigned char);
		unsigned char* Texels  = (unsigned char*) malloc(size);
		for (int i = 0; i < size; i++) 
		{
			Texels[i] = ((i % 4) < 3 ) ? (unsigned char) fgetc(file) : (unsigned char) 255 ;
		}
		fclose(file);
		return Texels;
	}
}

*/

using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class LoaderPPM : MonoBehaviour 
{
	public string path;
	public int size;
	
	[DllImport("LoadPPM", EntryPoint = "LoadPPM")]
	static extern IntPtr LoadPPM([MarshalAs(UnmanagedType.LPStr)] string s);
	
	Texture2D FlipTexture(Texture2D source)
	{
		int x = source.width;
		int y = source.height;
		Texture2D target = new Texture2D(x,y);                
		for(int i=0; i<x; i++)
		{
			for(int j=0; j<y; j++)
			{
				target.SetPixel(i, y-j-1, source.GetPixel(i,j));
			}
		}
		target.Apply();        
		return target;
	}

	void Start () 
	{
		IntPtr handle = LoadPPM(path);
		byte[] buffer = new byte[size*size*4];
		Marshal.Copy(handle, buffer, 0, size*size*4);
		Texture2D image = new Texture2D(size,size, TextureFormat.RGBA32, false);
		image.LoadRawTextureData(buffer);
		image.Apply();
		image = FlipTexture(image);	
		GetComponent<Renderer>().material.mainTexture = image;	
	}
}