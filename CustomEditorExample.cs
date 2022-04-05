using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using System;

public class CustomEditorExample : MonoBehaviour
{

}

#if UNITY_EDITOR
[CustomEditor(typeof(CustomEditorExample))]
public class CustomEditorDrawer : Editor
{
	public float Amount = 5.0f;
	private int _Number = 10;
}

public class CustomEditorExampleEditor : EditorWindow
{
	[MenuItem("Assets/Custom Editor Example Editor")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow ( typeof(CustomEditorExampleEditor));
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Show Custom Editor Fields" ) )
		{
			Type type = Type.GetType("CustomEditorExample,Assembly-CSharp");
			Component component = Selection.activeGameObject.GetComponent(type);
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
			Type customType = GetCustomEditorType(component);
			FieldInfo[] infos = customType.GetFields(bindingFlags);
			for (int i = 0; i < infos.Length; i++) Debug.Log(infos[i].Name);
		}
	} 

	Type GetCustomEditorType(UnityEngine.Object source)
	{
		Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
		System.Object instance = assembly.CreateInstance("UnityEditor.CustomEditorAttributes");
		Type type = instance.GetType();
		BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.NonPublic;
		MethodInfo methodInfo = type.GetMethod("FindCustomEditorType", bindingFlags);
		return (Type)methodInfo.Invoke(instance, new object[] { source, false });
	}
}
#endif