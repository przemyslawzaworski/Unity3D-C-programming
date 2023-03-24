using UnityEngine;
using UnityEditor;
using System.Linq;

public class EditorBuild : EditorWindow
{
	[MenuItem("Assets/EditorBuild")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow (typeof(EditorBuild));
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Sort Scenes" ) ) SortScenes();
	} 

	void SortScenes()
	{
		EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes.OrderBy(ebs => ebs.path).ToArray();
		EditorBuildSettings.scenes = scenes;
	}
}