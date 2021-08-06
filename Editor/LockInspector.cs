using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;

public class LockInspector : EditorWindow
{
	[MenuItem("GameObject/Get Locked GameObject Name", false, 10)]
	public static void GetLockedGameObjectName()
	{
		GameObject lockedObject = GetLockedGameObject();
		Debug.Log(lockedObject.name);
	}

	static GameObject GetLockedGameObject() //return locked gameobject from Inspector Window or null value
	{
		Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
		EditorWindow window = EditorWindow.GetWindow(type);
		FieldInfo fieldInfo = type.GetField("m_Tracker", BindingFlags.NonPublic | BindingFlags.Instance);
		ActiveEditorTracker tracker = fieldInfo.GetValue(window) as ActiveEditorTracker;
		MethodInfo methodInfo = tracker.GetType().GetMethod("GetObjectsLockedByThisTracker", BindingFlags.NonPublic | BindingFlags.Instance);
		List<UnityEngine.Object> lockedObjects = new List<UnityEngine.Object>();
		methodInfo.Invoke(tracker, new object[] {lockedObjects} );
		return (lockedObjects.Count > 0) ? (GameObject) lockedObjects[0] : null;
	}
}