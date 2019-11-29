using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

public class TextureImport : EditorWindow
{
	float Percent = 0.0f;
	IEnumerator Routine = null;
	bool IsCrunch = false;
	bool KeepUncompressed = true;
	TextureImporterCompression compression = TextureImporterCompression.Compressed;
	
	[MenuItem("Assets/TextureImport")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow ( typeof(TextureImport));
	}

	void OnEnable ()
	{
		EditorApplication.update += HandleCallbackFunction;
	}
 
	void HandleCallbackFunction ()
	{
		if (Routine != null && !Routine.MoveNext()) Routine = null;
	}
 
	void OnDisable ()
	{
		EditorApplication.update -= HandleCallbackFunction;
	}	
	
	void OnInspectorUpdate()
	{
		Repaint();
	}
	
	void OnGUI()
	{
		IsCrunch = EditorGUILayout.Toggle("Is Crunched ?", IsCrunch);
		compression = (TextureImporterCompression)EditorGUILayout.EnumPopup("Compression:", compression);
		KeepUncompressed = EditorGUILayout.Toggle("Keep Uncompressed ?", KeepUncompressed);
		if ( GUILayout.Button( "Begin" ) ) Routine = Import();
		EditorGUILayout.LabelField("Progress (%): ", Percent.ToString("0.00"));
	} 

	IEnumerator Import() 
	{
		int quality = 50;
		var assets = AssetDatabase.FindAssets ("t:texture", new[] {"Assets"}).Select (o => AssetImporter.GetAtPath (AssetDatabase.GUIDToAssetPath(o)) as TextureImporter);
		var textures = assets.Where (o => o != null).Where (o => o.crunchedCompression != IsCrunch || o.textureCompression != compression);
		float progress = 0.0f;
		float total = (float)textures.Count();
		foreach (var textureImporter in textures)
		{
			if (!(KeepUncompressed && textureImporter.textureCompression==TextureImporterCompression.Uncompressed)) textureImporter.textureCompression = compression;
			textureImporter.crunchedCompression = IsCrunch;
			textureImporter.compressionQuality = quality;
			AssetDatabase.ImportAsset(textureImporter.assetPath);
			progress += 1.0f;
			Percent = progress / total * 100.0f;
			yield return null;
		}
		Routine = null;
	}
}