using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class TextureChannelEditor : EditorWindow
{
	[MenuItem("Tools/Texture Channel Editor")]
	static void ShowWindow () 
	{
		EditorWindow window = EditorWindow.GetWindow(typeof(TextureChannelEditor));
		window.minSize = new Vector2(900f, 900f);
	}

	public enum Channel
	{
		R, G, B, A
	}

	[SerializeField] Texture2D _Texture;
	ScriptableObject _ScriptableObject;
	SerializedObject _SerializedObject;
	SerializedProperty _TextureProperty;
	Channel _SourceChannel;
	Channel _DestinationChannel;
	bool _Zero = true;

	void OnEnable()
	{
		_ScriptableObject = this;
		_SerializedObject = new SerializedObject(_ScriptableObject);
		_TextureProperty = _SerializedObject.FindProperty("_Texture");
	}

	void OnGUI()
	{
		EditorGUILayout.PropertyField(_TextureProperty);
		_SourceChannel = (Channel)EditorGUILayout.EnumPopup("Source channel:", _SourceChannel);
		_DestinationChannel = (Channel)EditorGUILayout.EnumPopup("Destination channel:", _DestinationChannel);
		_Zero = EditorGUILayout.Toggle("Clear all channels", _Zero);
		if (_Texture != null && GUILayout.Button("Copy Channel"))
		{
			CopyChannel(_Texture, _SourceChannel, _DestinationChannel);
		}
		if (_Texture != null && GUILayout.Button("Save as PNG"))
		{
			SaveAsPNG(_Texture);
		}
		_SerializedObject.ApplyModifiedProperties();
	}

	void CopyChannel(Texture2D sourceTexture, Channel sourceChannel, Channel destinationChannel)
	{
		Texture2D texture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, false);
		Color[] pixels = sourceTexture.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			Color pixel = pixels[i];
			float sourceValue = GetChannelValue(pixel, sourceChannel);
			if (_Zero) pixel = Color.clear;
			pixels[i] = SetChannelValue(pixel, destinationChannel, sourceValue);
		}
		texture.SetPixels(pixels);
		texture.Apply();
		_Texture = texture;
	}

	float GetChannelValue(Color color, Channel channel)
	{
		switch (channel)
		{
			case Channel.R: return color.r;
			case Channel.G: return color.g;
			case Channel.B: return color.b;
			case Channel.A: return color.a;
			default: return 0f;
		}
	}

	Color SetChannelValue(Color color, Channel channel, float value)
	{
		switch (channel)
		{
			case Channel.R: color.r = value; break;
			case Channel.G: color.g = value; break;
			case Channel.B: color.b = value; break;
			case Channel.A: color.a = value; break;
		}
		return color;
	}

	void SaveAsPNG(Texture2D texture)
	{
		string path = EditorUtility.SaveFilePanel("Save texture as PNG", "", texture.name + ".png", "png");
		if (!string.IsNullOrEmpty(path))
		{
			byte[] bytes = texture.EncodeToPNG();
			System.IO.File.WriteAllBytes(path, bytes);
			AssetDatabase.Refresh();
		}
	}

	void OnDestroy()
	{
		if (_Texture != null) DestroyImmediate(_Texture);
	}
}