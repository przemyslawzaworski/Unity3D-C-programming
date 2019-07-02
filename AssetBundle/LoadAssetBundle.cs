// Example script
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAssetBundle : MonoBehaviour 
{
	public string AssetBundlePackName;
	public string[] PrefabNames;
	
	void Start () 
	{
		var Archive = AssetBundle.LoadFromFile(Application.streamingAssetsPath+"/"+AssetBundlePackName);
		if (Archive == null) 
		{
			Debug.Log("Failed to load AssetBundle!");
			return;
		}
		for (int i=0; i<PrefabNames.Length; i++)
		{
			var source = Archive.LoadAsset<GameObject>(PrefabNames[i]);
			Instantiate(source);
		}		
	}
}