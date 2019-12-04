using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class AddressableManagerAsync : MonoBehaviour
{
	GameObject _Terrain;
	
	async Task<GameObject> LoadAssetInstance (string path) 
	{
		IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> locations = await Addressables.LoadResourceLocationsAsync(path).Task;
		if (locations.Count == 0) return null;
		AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(locations[0]);
		await handle.Task;
		return handle.Result;
	}

	void UnloadAssetInstance (GameObject instance)
	{
		Addressables.ReleaseInstance(instance);
	}
	
	async void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{
			if (_Terrain == null) _Terrain = await LoadAssetInstance("Assets/Prefabs/Rock01.prefab");
		}
		else if (Input.GetKeyDown(KeyCode.P))
		{	
			if (_Terrain != null) UnloadAssetInstance(_Terrain);
		}
	}
}