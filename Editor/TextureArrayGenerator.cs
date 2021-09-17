// In generated Texture Array import settings (inspector), set Color Space to 1 (linear)
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class TextureArrayGenerator : EditorWindow
{
	[MenuItem("Assets/Texture Array Generator")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow ( typeof(TextureArrayGenerator));
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Generate" ) ) Generate();
	} 

	void Generate()
	{
		int width = 1;
		int height = 1;
		int depth = 1;
		List<Texture2D> textures = new List<Texture2D>();
		foreach (UnityEngine.Object source in Selection.objects)
		{
			Texture2D texture = source as Texture2D;
			if (texture)
			{
				textures.Add(texture);
				width = Mathf.Max(width, texture.width);
				height = Mathf.Max(height, texture.height);
			}
		}
		depth = Selection.objects.Length;
		Texture2DArray textureArray = new Texture2DArray( width, height, depth, TextureFormat.ARGB32, true, true );
		textureArray.Apply( false );
		RenderTexture cache = RenderTexture.active;
		RenderTexture renderTexture = new RenderTexture( width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default );
		renderTexture.Create();
		for( int i = 0; i < depth; i++ )
		{
			RenderTexture.active = renderTexture;
			Graphics.Blit( textures[i], renderTexture );
			Texture2D texture = new Texture2D( width, height, TextureFormat.ARGB32, true, true );
			texture.ReadPixels( new Rect( 0, 0, width, height ), 0, 0, true );
			RenderTexture.active = null;
			int maxSize = Mathf.Max( width, height );
			int mipCount = System.Convert.ToInt32(Mathf.Log(maxSize, 2) + 1.0f);
			for( int m = 0; m < mipCount; m++ )
			{
				textureArray.SetPixels( texture.GetPixels(), i, m );
				textureArray.Apply();
			}
			Destroy(texture);
		}
		renderTexture.Release();
		RenderTexture.active = cache;
		AssetDatabase.CreateAsset(textureArray, "Assets/TextureArray.asset");
	}
}