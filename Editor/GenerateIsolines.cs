using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Data;
using System.Linq;

public class GenerateIsolines : EditorWindow
{
	[MenuItem("Tools/Generate Isolines")]
	static void ShowWindow () 
	{
		EditorWindow window = EditorWindow.GetWindow (typeof(GenerateIsolines));
		window.minSize = new Vector2(900f, 900f);
	}

	[SerializeField] Texture2D _Texture;
	[SerializeField] float _SampleBase = 0.05f;
	[SerializeField] float _SampleDetail = 0.005f;
	[SerializeField] bool _Inverted = false;

	Material _Material;	
	RenderTexture _RenderTexture;
	ScriptableObject _ScriptableObject;
	SerializedObject _SerializedObject;
	SerializedProperty _TextureProperty;

	void OnEnable()
	{
		_ScriptableObject = this;
		_SerializedObject = new SerializedObject (_ScriptableObject);
		_TextureProperty = _SerializedObject.FindProperty ("_Texture");
		_Material = new Material(Shader.Find("Hidden/GenerateIsolines"));
		_RenderTexture = new RenderTexture(2048, 2048, 16, RenderTextureFormat.ARGB32);
		_RenderTexture.Create();
	}

	void OnGUI()
	{
		EditorGUILayout.PropertyField(_TextureProperty);
		GUI.Label(new Rect(10, 60, 80, 20), "Isoline Base");
		_SampleBase = GUI.HorizontalSlider(new Rect(100, 60, 100, 30), _SampleBase, 0.0001F, 0.5F);
		_Material.SetFloat("_SampleBase", _SampleBase);
		GUI.Label(new Rect(10, 80, 80, 20), "Isoline Detail");
		_SampleDetail = GUI.HorizontalSlider(new Rect(100, 80, 100, 30), _SampleDetail, 0.0001F, 0.5F);
		_Material.SetFloat("_SampleDetail", _SampleDetail);
		_Inverted = EditorGUILayout.Toggle("Color Inversion", _Inverted);
		_Material.SetFloat("_Inverted", Convert.ToSingle(_Inverted));
		if ( GUILayout.Button( "Save as PNG" ) )
		{
			Graphics.Blit(_Texture, _RenderTexture, _Material, 0);
			RenderTexture.active = _RenderTexture;
			Texture2D texture = new Texture2D(_RenderTexture.width, _RenderTexture.height, TextureFormat.ARGB32, false);
			texture.ReadPixels(new Rect(0, 0, _RenderTexture.width, _RenderTexture.height), 0, 0);
			RenderTexture.active = null;
			byte[] bytes = texture.EncodeToPNG();  
			string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Isolines.png");
			System.IO.File.WriteAllBytes(filePath, bytes); 
			System.Diagnostics.Process.Start(filePath);
		}
		if (Event.current.type.Equals(EventType.Repaint))
		{
			if (_Texture != null && _RenderTexture != null)
			{
				GUI.DrawTexture(new Rect(10, 110, 768, 768), _RenderTexture, ScaleMode.ScaleToFit, false, 0);
				Graphics.Blit(_Texture, _RenderTexture, _Material, 0);
			}
		}
		_SerializedObject.ApplyModifiedProperties();
		Repaint();
	}

	void OnDisable()
	{
		if (_Material != null) DestroyImmediate(_Material);
		if (_RenderTexture != null) DestroyImmediate(_RenderTexture);
	}
}