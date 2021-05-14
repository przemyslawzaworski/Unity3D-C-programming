// Install "Addressable Asset System" package.
// Select texture as "Addressable".
// Window -> Asset Management -> Addressables -> Groups -> Build -> Build Player Content
// To measure memory usage in Editor, set "Window -> Asset Management -> Addressables -> Groups -> Play Mode Script -> Use Existing Build"
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManagerTextures : MonoBehaviour
{
	public AssetReferenceTexture2D Texture;
	private Texture2D _Texture;
	private GameObject _Plane;

	public async Task<Texture2D> LoadTexture (AssetReferenceTexture2D addressable)
	{
		AsyncOperationHandle<Texture2D> handle = addressable.LoadAssetAsync();
		await handle.Task;
		return handle.Result;
	}

	public void ReleaseTexture (AssetReferenceTexture2D addressable)
	{
		addressable.ReleaseAsset();
	}

	void Start()
	{
		_Plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		_Plane.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
	}

	async void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{
			if (_Texture == null)
			{
				_Texture = await LoadTexture(Texture);
				_Plane.GetComponent<Renderer>().material.mainTexture = _Texture;
			}
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			if (_Texture != null)
			{
				_Plane.GetComponent<Renderer>().material.mainTexture = null;
				_Texture = null;
				ReleaseTexture(Texture);
			}
		}
	}
}