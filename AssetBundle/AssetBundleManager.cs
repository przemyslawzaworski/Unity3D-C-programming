using UnityEngine;

public class AssetBundleManager : MonoBehaviour
{
	// structure for storing prefab data
	struct Data
	{
		public AssetBundle Bundle;
		public GameObject Instance;
	}

	// container
	Data _Container;

	// load prefab to memory
	Data LoadAssetInstance(string path, string address)
	{
		AssetBundle bundle = AssetBundle.LoadFromFile(path);
		GameObject instance = Instantiate(bundle.LoadAsset<GameObject>(address));
		Data data = new Data();
		data.Bundle = bundle;
		data.Instance = instance;
		return data;
	}

	// unload prefab from memory
	void UnloadAssetInstance (Data data)
	{
		Destroy(data.Instance);
		data.Bundle.Unload(true);
		Resources.UnloadUnusedAssets();
	}

	// example usage:
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{	
			_Container = LoadAssetInstance("D:\\Mods\\test_group_assets_all_7226dcfc3174ecd7d6182fefc9cc0273.bundle", "Assets/Prefabs/Sphere.prefab");	
		}
		if (Input.GetKeyDown(KeyCode.P))
		{	
			UnloadAssetInstance(_Container);
		}
	}
}