using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CustomSphere : ScriptableObject
{
	public Vector3 Position;
	public float Radius;
}

public class ScriptableObjectsManager : EditorWindow
{	
	[MenuItem("Assets/Scriptable Objects Manager")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow ( typeof(ScriptableObjectsManager));
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Load Data" ) ) LoadData();
		if ( GUILayout.Button( "Save Data" ) ) SaveData();
	} 

	void LoadData()
	{
		string path = EditorUtility.OpenFilePanel("Load file", "", "asset");
		if (path.Length != 0)
		{
			string[] split = path.Split(new [] {"Assets"}, System.StringSplitOptions.None);
			string filePath = "Assets" + split[split.Length - 1];
			CustomSphere scriptableObject = AssetDatabase.LoadAssetAtPath(filePath, typeof(CustomSphere)) as CustomSphere;
			Debug.Log(scriptableObject.Position);
			Debug.Log(scriptableObject.Radius);
		}
	}

	void SaveData()
	{
		CustomSphere scriptableObject = ScriptableObject.CreateInstance(typeof(CustomSphere)) as CustomSphere;
		scriptableObject.Position = new Vector3(10f, 20f, 30f);
		scriptableObject.Radius = 2.0f;
		string filePath = EditorUtility.SaveFilePanelInProject("Save file", "CustomSphere.asset", "asset", "");
		if (filePath.Length != 0)
		{
			AssetDatabase.CreateAsset(scriptableObject, filePath);
			AssetDatabase.Refresh();
		}
	}
}
