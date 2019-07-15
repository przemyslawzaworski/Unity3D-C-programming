// Install "Addressable Asset System" package.
// Select prefab as "Addressable".
// Window -> Asset Management -> Addressables -> Play Mode Script -> Packed Play Mode
// Window -> Asset Management -> Addressables -> Build -> Build Player Content
//using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
	public static AddressableManager Instance;

	// structure for storing prefab data
	public struct Data
	{
		public GameObject asset;
		public GameObject instance;
	}
	
	// container
	public Data _PlasmaCube;
	
	// load prefab to memory asynchronously
	public async Task<Data> LoadAssetInstance(string location)
	{
		AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(location);
		await handle.Task;
		Data data = new Data();
		data.asset = handle.Result;
		data.instance = Instantiate(data.asset);
		return data;
	}
	
	// unload prefab from memory
	public void UnloadAssetInstance (Data obj)
	{
		if(!Addressables.ReleaseInstance(obj.instance))
		{
			Destroy(obj.instance);
			Addressables.Release(obj.asset);
		}
	}
	
	void Awake()
	{
		Instance = this;
	}
	
	// example usage:
	async void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{	
			_PlasmaCube = await LoadAssetInstance("Assets/PlasmaCube.prefab");	
		}
		if (Input.GetKeyDown(KeyCode.P))
		{	
			UnloadAssetInstance(_PlasmaCube);
		}
	}
}
