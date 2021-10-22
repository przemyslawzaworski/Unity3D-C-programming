using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

public class ResolutionManager : EditorWindow
{
	[MenuItem("Assets/Resolution Manager")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow (typeof(ResolutionManager));
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Add and set new resolution" ) )
		{
			AddResolution(256, 256, "Screenshots");
			SetResolution(GetCount() - 1);
		}
		if ( GUILayout.Button( "Restore to default resolution" ) )
		{
			SetResolution(0);
			RemoveResolution(GetCount() - 1);
		}
	}

	void AddResolution(int width, int height, string label)
	{
		Type gameViewSize = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
		Type gameViewSizes = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
		Type gameViewSizeType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
		Type generic = typeof(ScriptableSingleton<>).MakeGenericType(gameViewSizes);
		MethodInfo getGroup = gameViewSizes.GetMethod("GetGroup");
		object instance = generic.GetProperty("instance").GetValue(null, null);		  
		object group = getGroup.Invoke(instance, new object[] { (int)GameViewSizeGroupType.Standalone });		
		Type[] types = new Type[] { gameViewSizeType, typeof(int), typeof(int), typeof(string)};
		ConstructorInfo constructorInfo = gameViewSize.GetConstructor(types);
		object entry = constructorInfo.Invoke(new object[] { 1, width, height, label });
		MethodInfo addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize");
		addCustomSize.Invoke(group, new object[] { entry });
	}

	void RemoveResolution(int index)
	{
		Type gameViewSizes = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
		Type generic = typeof(ScriptableSingleton<>).MakeGenericType(gameViewSizes);
		MethodInfo getGroup = gameViewSizes.GetMethod("GetGroup");
		object instance = generic.GetProperty("instance").GetValue(null, null);
		object group = getGroup.Invoke(instance, new object[] { (int)GameViewSizeGroupType.Standalone });
		MethodInfo removeCustomSize = getGroup.ReturnType.GetMethod("RemoveCustomSize");
		removeCustomSize.Invoke(group, new object[] { index });
	}

	int GetCount()
	{
		Type gameViewSizes = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
		Type generic = typeof(ScriptableSingleton<>).MakeGenericType(gameViewSizes);
		MethodInfo getGroup = gameViewSizes.GetMethod("GetGroup");
		object instance = generic.GetProperty("instance").GetValue(null, null);
		PropertyInfo currentGroupType = instance.GetType().GetProperty("currentGroupType");
		GameViewSizeGroupType groupType = (GameViewSizeGroupType)(int)currentGroupType.GetValue(instance, null);
		object group = getGroup.Invoke(instance, new object[] { (int)groupType });
		MethodInfo getBuiltinCount = group.GetType().GetMethod("GetBuiltinCount");
		MethodInfo getCustomCount = group.GetType().GetMethod("GetCustomCount");
		return (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);	
	}

	void SetResolution(int index)
	{
		Type gameView = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
		PropertyInfo selectedSizeIndex = gameView.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		EditorWindow window = EditorWindow.GetWindow(gameView);
		selectedSizeIndex.SetValue(window, index, null);
	}
}