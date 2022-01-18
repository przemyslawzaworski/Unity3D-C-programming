using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using System.IO;

public class AssetBundleAsyncAwait : MonoBehaviour
{
	public async Task<Texture2D> LoadTextureAsync(string filePath, string address)
	{
		AssetBundleCreateRequest handle = await AssetBundle.LoadFromFileAsync(filePath);
		AssetBundleRequest request = await handle.assetBundle.LoadAssetAsync<Texture2D>(address);
		Texture2D texture = request.asset as Texture2D;
		return texture;
	}

	async void Start()
	{
		Texture2D texture = await LoadTextureAsync(Path.Combine(Application.streamingAssetsPath, "test.bundle"), "Assets/icon.png");	
		GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		plane.GetComponent<Renderer>().material.mainTexture = texture;
	}
}

// https://docs.unity3d.com/ScriptReference/AssetBundleCreateRequest.html
public class AssetBundleCreateRequestAwaiter : INotifyCompletion
{
	readonly AssetBundleCreateRequest _Request;
	Action _Action;

	public AssetBundleCreateRequestAwaiter(AssetBundleCreateRequest request)
	{
		this._Request = request;
		request.completed += OnRequestCompleted;
	}

	public bool IsCompleted { get { return _Request.isDone; } }

	public AssetBundleCreateRequest GetResult() { return this._Request; }

	public void OnCompleted(Action action)
	{
		this._Action = action;
	}

	private void OnRequestCompleted(AsyncOperation command)
	{
		_Action();
	}
}

public static class AssetBundleCreateRequestAsync
{
	public static AssetBundleCreateRequestAwaiter GetAwaiter(this AssetBundleCreateRequest request)
	{
		return new AssetBundleCreateRequestAwaiter(request);
	}
}

// https://docs.unity3d.com/ScriptReference/AssetBundleRequest.html
public class AssetBundleRequestAwaiter : INotifyCompletion
{
	readonly AssetBundleRequest _Request;
	Action _Action;

	public AssetBundleRequestAwaiter(AssetBundleRequest request)
	{
		this._Request = request;
		request.completed += OnRequestCompleted;
	}

	public bool IsCompleted { get { return _Request.isDone; } }

	public AssetBundleRequest GetResult() { return this._Request; }

	public void OnCompleted(Action action)
	{
		this._Action = action;
	}

	private void OnRequestCompleted(AsyncOperation command)
	{
		_Action();
	}
}

public static class AssetBundleRequestAsync
{
	public static AssetBundleRequestAwaiter GetAwaiter(this AssetBundleRequest request)
	{
		return new AssetBundleRequestAwaiter(request);
	}
}
