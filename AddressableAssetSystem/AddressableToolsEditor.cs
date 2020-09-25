using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.ResourceManagement.Util;

public class AddressableToolsEditor : EditorWindow
{
	[MenuItem("Assets/Addressable Tools")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow ( typeof(AddressableToolsEditor) );
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Show All Labels" ) ) 
		{
			List<string> labels = GetAllLabels();
			for ( int i = 0; i < labels.Count; i++ ) Debug.Log(labels[i]);
		}
		if ( GUILayout.Button( "Show All Addresses" ) ) 
		{
			List<string> addresses = GetAllAddresses();
			for ( int i = 0; i < addresses.Count; i++ ) Debug.Log(addresses[i]);
		}
		if ( GUILayout.Button( "Show All Groups" ) ) 
		{
			List<string> groups = GetAllGroups();
			for ( int i = 0; i < groups.Count; i++ ) Debug.Log(groups[i]);
		}
		if ( GUILayout.Button( "Create Dictionary" ) ) 
		{
			Dictionary<string, List<string>> dictionary = CreateDictionary();
			foreach (KeyValuePair<string, List<string>> pair in dictionary)
			{
				List<string> list = pair.Value;
				for (int j = 0; j < list.Count; j++) Debug.Log(pair.Key.ToString() + " ### " + list[j]);
			}
		}
		if ( GUILayout.Button( "Create Group" ) ) CreateGroup("Test");
		if ( GUILayout.Button( "Create Entry" ) ) CreateEntry("Assets/Prefabs/Sphere.prefab", "Test", "default");
		if ( GUILayout.Button( "Remove Group" ) ) RemoveGroup("Test");
		if ( GUILayout.Button( "Remove Entry" ) ) RemoveEntry("Assets/Prefabs/Sphere.prefab");
	}

	List<string> GetAllLabels()
	{
		string guid = AssetDatabase.FindAssets( "t:AddressableAssetSettings" ).FirstOrDefault();
		string path = AssetDatabase.GUIDToAssetPath( guid );
		AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>( path );
		SerializedObject serializedObject = new SerializedObject( settings );
		SerializedProperty table = serializedObject.FindProperty( "m_LabelTable" );
		SerializedProperty names = table.FindPropertyRelative( "m_LabelNames" );
		List<string> result = new List<string>();
		for (int i = 0; i < names.arraySize; i++) result.Add( names.GetArrayElementAtIndex(i).stringValue ); 
		return result;
	}

	List<string> GetAllAddresses()
	{
		string guid = AssetDatabase.FindAssets( "t:AddressableAssetSettings" ).FirstOrDefault();
		string path = AssetDatabase.GUIDToAssetPath( guid );
		AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>( path );
		List<AddressableAssetEntry> entries = new List<AddressableAssetEntry>();
		settings.GetAllAssets( entries, true );	
		Regex regex = new Regex( @"(.*)\[.*\]" );
		return entries.Select( c => c.address ).Select( c => regex.Replace( c, "$1" ) ).GroupBy( c => c ).Select( c => c.Key ).ToList();
	}

	List<string> GetAllGroups()
	{
		string guid = AssetDatabase.FindAssets( "t:AddressableAssetSettings" ).FirstOrDefault();
		string path = AssetDatabase.GUIDToAssetPath( guid );
		AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>( path );
		return settings.groups.Select( c => c.Name ).ToList();
	}

	List<AddressableAssetGroupSchema> GetAllSchemas<AddressableAssetGroupSchema>()
	{
		string guid = AssetDatabase.FindAssets( "t:AddressableAssetSettings" ).FirstOrDefault();
		string path = AssetDatabase.GUIDToAssetPath( guid );
		AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>( path );
		return settings.groups.SelectMany( c => c.Schemas ).OfType<AddressableAssetGroupSchema>().ToList();
	}

	Dictionary<string, List<string>> CreateDictionary()
	{
		string guid = AssetDatabase.FindAssets( "t:AddressableAssetSettings" ).FirstOrDefault();
		string path = AssetDatabase.GUIDToAssetPath( guid );
		AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>( path );
		Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
		List<AddressableAssetGroup> groups = settings.groups;
		for (int i = 0; i < groups.Count; i++)
		{
			List<AddressableAssetEntry> entries = groups[i].entries.ToList();
			List<string> list = new List<string>();
			for (int j = 0; j < entries.Count; j++) list.Add(entries[j].AssetPath.ToString());
			dictionary.Add(groups[i].Name, list);
		}
		return dictionary;
	}

	AddressableAssetGroup CreateGroup(string groupName)
	{
		string guid = AssetDatabase.FindAssets( "t:AddressableAssetSettings" ).FirstOrDefault();
		string path = AssetDatabase.GUIDToAssetPath( guid );
		AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>( path );
		BundledAssetGroupSchema bundledAssetGroupSchema = ScriptableObject.CreateInstance<BundledAssetGroupSchema>();
		ContentUpdateGroupSchema contentUpdateGroupSchema = ScriptableObject.CreateInstance<ContentUpdateGroupSchema>();
		var schemas = new List<AddressableAssetGroupSchema>{bundledAssetGroupSchema, contentUpdateGroupSchema};
		return settings.CreateGroup(groupName, false, false, true, schemas);
	}

	AddressableAssetEntry CreateEntry(string filePath, string groupName, string labelName)
	{
		string path = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets( "t:AddressableAssetSettings" ).FirstOrDefault());
		AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>( path );
		AddressableAssetGroup target = settings.groups.Find( c => c.name == groupName );
		string guid = AssetDatabase.AssetPathToGUID(filePath);
		AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, target, false, true);
		entry.SetAddress(filePath, true);
		entry.SetLabel(labelName, true, true);
		return entry;
	}

	void RemoveGroup(string groupName)
	{
		string guid = AssetDatabase.FindAssets( "t:AddressableAssetSettings" ).FirstOrDefault();
		string path = AssetDatabase.GUIDToAssetPath( guid );
		AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>( path );
		AddressableAssetGroup target = settings.groups.Find( c => c.name == groupName );
		settings.RemoveGroup( target );	
	}

	void RemoveEntry(string filePath)
	{
		string path = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets( "t:AddressableAssetSettings" ).FirstOrDefault());
		AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>( path );
		string guid = AssetDatabase.AssetPathToGUID(filePath);
		settings.RemoveAssetEntry(guid, true);		
	}	
}