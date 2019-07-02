using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateAssetBundles : EditorWindow
{
	string AssetBundleName = "";
	string assetBundleDirectory = "Assets/StreamingAssets";
	
	[MenuItem("Assets/CreateAssetBundles")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow ( typeof(CreateAssetBundles));
	}

	void OnGUI()
	{
		AssetBundleName = EditorGUILayout.TextField("Bundle name: ", AssetBundleName);
		if ( GUILayout.Button( "Generate" ) ) BuildAllAssetBundles();
	} 
		
	void BuildAllAssetBundles()
	{
		string[] AssetNames = new string[Selection.transforms.Length];
		
		for (int i=0; i<AssetNames.Length; i++)
		{
			GameObject prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(Selection.transforms[i].gameObject);
			UnityEngine.Object source = null;
			if (prefab != null)
			{ 
				source = PrefabUtility.GetCorrespondingObjectFromSource(prefab); 
			}
			AssetNames[i] = AssetDatabase.GetAssetPath(source);
		}
		
		AssetBundleBuild[] bundles = new AssetBundleBuild[]
		{
			new AssetBundleBuild()
			{
				assetBundleName = AssetBundleName,
				assetNames = AssetNames
			}
		};

		if(!Directory.Exists(assetBundleDirectory))
		{
			Directory.CreateDirectory(assetBundleDirectory);
		}
		BuildPipeline.BuildAssetBundles(assetBundleDirectory,bundles,BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
	}
}