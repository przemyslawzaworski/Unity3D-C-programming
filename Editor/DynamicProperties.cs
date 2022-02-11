using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Reflection;
using System.Reflection.Emit;

public class DynamicProperties : EditorWindow
{
	[MenuItem("Assets/Dynamic Properties Demo")]
	static void ShowWindow() 
	{
		DynamicProperties window = EditorWindow.GetWindow<DynamicProperties>();
		window.titleContent = new GUIContent("Dynamic Properties Demo");
		window.minSize = new Vector2(200f, 600f);
	}

	private BindingFlags _BindingFlags = BindingFlags.Instance | BindingFlags.Public;
	private dynamic _ScriptableObject;
	private SerializedObject _SerializedObject;
	private List<SerializedProperty> _SerializedProperties = new List<SerializedProperty>();
	private Enum _Enum, _Cache;

	void GenerateProperties()
	{
		_SerializedObject = new SerializedObject(_ScriptableObject);
		_SerializedProperties = new List<SerializedProperty>();
		FieldInfo[] infos = _ScriptableObject.GetType().GetFields(_BindingFlags);
		foreach (var info in infos) _SerializedProperties.Add(_SerializedObject.FindProperty(info.Name));
	}

	void OnEnable()
	{
		Assembly assembly = Assembly.GetExecutingAssembly();
		Type[] types = assembly.GetTypes();
		List<string> list = new List<string>();
		foreach (var type in types) if (type.IsSerializable) list.Add(type.FullName);
		_Enum = (System.Enum)Activator.CreateInstance(GenerateEnumeration(list));
		_ScriptableObject = ScriptableObject.CreateInstance(Type.GetType(_Enum.ToString()));
		GenerateProperties();
	}

	void OnGUI()
	{
		_Enum = (System.Enum)EditorGUILayout.EnumPopup(_Enum);
		if (_Enum != _Cache)
		{
			_ScriptableObject = ScriptableObject.CreateInstance(Type.GetType(_Enum.ToString()));
			GenerateProperties();
		}
		_Cache = _Enum;
		if (GUILayout.Button( "Open file" ))
		{
			string path = EditorUtility.OpenFilePanel("Load JSON", "", "json");
			if (path.Length != 0)
			{
				StreamReader reader = new StreamReader(path);
				string json = reader.ReadToEnd();
				reader.Close();
				Type type = Type.GetType(_Enum.ToString());
				_ScriptableObject = ScriptableObject.CreateInstance(type);
				JsonUtility.FromJsonOverwrite(json, _ScriptableObject);
				GenerateProperties();
			}
		}
		if (GUILayout.Button( "Save file" ))
		{
			string path = EditorUtility.SaveFilePanel("Save JSON","","test.json","json");
			if (path.Length != 0)
			{
				string json = JsonUtility.ToJson(_ScriptableObject, true);
				StreamWriter writer = new StreamWriter(path);
				writer.Write(json);
				writer.Close();
			}
		}
		for (int i = 0; i < _SerializedProperties.Count; i++) EditorGUILayout.PropertyField(_SerializedProperties[i]);
		if (_SerializedObject != null) _SerializedObject.ApplyModifiedProperties();
	}

	Type GenerateEnumeration(List<string> list)
	{
		AppDomain appDomain = AppDomain.CurrentDomain;
		AssemblyName assemblyName = new AssemblyName("TempAssembly");
		AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
		ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
		EnumBuilder enumBuilder = moduleBuilder.DefineEnum("ClassName", TypeAttributes.Public, typeof(int));
		for (int i = 0; i < list.Count; i++) enumBuilder.DefineLiteral(list[i], i);
		return enumBuilder.CreateType();
	}
}

[Serializable]
class Soldier : ScriptableObject
{
	public string Name = "Default";
	public int Health = 100;
}

[Serializable]
class Weapon : ScriptableObject
{
	public float Accuracy = 5.0f;
	public float Damage = 2.0f;
	public List<string> Upgrades = new List<string>();
}