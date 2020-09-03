// Example how to set values for public fields automatically from data saved in configuration file.
// First usage: fill values manually, then click "Save References". Next time, for example when fields are empty,
// click "Load References". It can be useful for situations, when script has dozens of public fields.
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class FieldManager : EditorWindow
{
	[MenuItem("Assets/Field Manager")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow ( typeof(FieldManager) );
	}

	public GameObject Element;
	public Texture2D AlbedoMap;
	public Texture2D NormalMap;
	public float Amount;

	private ScriptableObject _ScriptableObject;	
	private SerializedObject _SerializedObject;
	private SerializedProperty _ElementProperty;
	private SerializedProperty _AlbedoMapProperty;
	private SerializedProperty _NormalMapProperty;
	private SerializedProperty _AmountProperty;
	private Vector2 _ScrollPos = Vector2.zero;

	[System.Serializable]
	struct Metadata
	{
		public string Type;
		public string Name;
		public string Value;
	};

	void OnEnable()
	{
		_ScriptableObject = this;
		_SerializedObject = new SerializedObject (_ScriptableObject);
		_ElementProperty = _SerializedObject.FindProperty ("Element");
		_AlbedoMapProperty = _SerializedObject.FindProperty ("AlbedoMap");
		_NormalMapProperty = _SerializedObject.FindProperty ("NormalMap");
		_AmountProperty = _SerializedObject.FindProperty ("Amount");
	}

	void OnGUI()
	{
		EditorGUILayout.BeginVertical();
		_ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
		EditorGUILayout.PropertyField(_ElementProperty);
		EditorGUILayout.PropertyField(_AlbedoMapProperty);
		EditorGUILayout.PropertyField(_NormalMapProperty);
		EditorGUILayout.PropertyField(_AmountProperty);	
		if ( GUILayout.Button( "Load References" ) ) LoadReferences();
		if ( GUILayout.Button( "Save References" ) ) SaveReferences();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	} 

	void Serialize (string path, List<Metadata> source)
	{
		try
		{
			BinaryFormatter bin = new BinaryFormatter();
			FileStream writer = new FileStream(path,FileMode.Create);
			bin.Serialize(writer, (object)source);
			writer.Close();
		}
		catch (IOException) {}
	}

	List<Metadata> Deserialize (string path)
	{
		FileStream reader = new FileStream(path, FileMode.Open, FileAccess.Read);
		BinaryFormatter bin = new BinaryFormatter();
		List<Metadata> target = (List<Metadata>) bin.Deserialize(reader);
		reader.Close();
		return target;
	}

	void LoadReferences(bool debug = false)
	{
		FieldInfo[] infos = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
		string path = Path.Combine(Application.streamingAssetsPath, "FieldManager.data");
		if (!File.Exists(path)) return;
		List<Metadata> metadata = Deserialize (path);
		for (int i = 0; i < metadata.Count; i++)
		{
			if (metadata[i].Type == "UnityEngine.GameObject")
			{
				string[] guids = AssetDatabase.FindAssets(metadata[i].Value + " t:prefab ", new[] {"Assets"});
				GameObject gameObject = (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(GameObject));
				infos[i].SetValue(this, gameObject);
			}
			else if (metadata[i].Type == "UnityEngine.Texture2D")
			{
				string[] guids = AssetDatabase.FindAssets(metadata[i].Value + " t:texture ", new[] {"Assets"});
				Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(Texture2D));
				infos[i].SetValue(this, texture);
			}
			else if (metadata[i].Type == "System.Single")
			{
				infos[i].SetValue(this, float.Parse(metadata[i].Value));
			}
			if (debug) Debug.Log(metadata[i].Type + " - " + metadata[i].Name + " - " + metadata[i].Value);
		}
		OnEnable();
	}

	void SaveReferences()
	{
		_SerializedObject.ApplyModifiedProperties();
		List<Metadata> metadata = new List<Metadata>();
		FieldInfo[] infos = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
		for (int i = 0; i < infos.Length; i++)
		{
			try
			{
				Metadata data = new Metadata();
				data.Type = infos[i].FieldType.ToString();
				data.Name = infos[i].Name.ToString();
				string chars = infos[i].GetValue(this).ToString(); 
				data.Value = chars.Contains("(") ? chars.Substring(0, chars.IndexOf("(")) : chars;
				metadata.Add(data);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}
		Serialize (Path.Combine(Application.streamingAssetsPath, "FieldManager.data"), metadata);
	}
}