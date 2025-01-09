using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.EditorCoroutines.Editor;

public class EditorCoroutines : EditorWindow
{
	[MenuItem("Assets/Editor Coroutines")]
	static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(EditorCoroutines));
	}

	EditorCoroutine _EditorCoroutine;

	void OnGUI()
	{
		if (GUILayout.Button("Read TXT file")) ReadFile();
	}

	void ReadFile()
	{
		string path = EditorUtility.OpenFilePanel("Open txt", "", "txt");
		if (path.Length != 0)
		{
			string[] lines = File.ReadAllLines(path);
			_EditorCoroutine = EditorCoroutineUtility.StartCoroutine(ReadLines(lines), this);
		}
	}

	IEnumerator ReadLines(string[] lines)
	{
		for (int i = 0; i < lines.Length; i++)
		{
			Debug.LogError(lines[i]);
			yield return new EditorWaitForSeconds(1.0f);
		}
	}

	void OnDisable()
	{
		this.StopCoroutine(_EditorCoroutine);
		_EditorCoroutine = null;
	}
}