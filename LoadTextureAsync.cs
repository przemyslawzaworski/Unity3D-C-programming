using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Runtime.CompilerServices;

public class UnityWebRequestAwaiter : INotifyCompletion
{
	UnityWebRequestAsyncOperation request;
	Action action; 

	public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation request)
	{
		this.request = request;
		request.completed += OnRequestCompleted;
	}

	public bool IsCompleted { get { return request.isDone; } }

	public void GetResult() { }

	public void OnCompleted(Action action)
	{
		this.action = action;
	}

	private void OnRequestCompleted(AsyncOperation command)
	{
		action();
	}
}

public static class ExtensionMethods
{
	public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation request)
	{
		return new UnityWebRequestAwaiter(request);
	}
}

public class LoadTextureAsync : MonoBehaviour
{
	async void Load (Material material, string property, string filepath)
	{
		UnityWebRequest request = UnityWebRequestTexture.GetTexture(filepath);
		await request.SendWebRequest();
		material.SetTexture(property, ((DownloadHandlerTexture)request.downloadHandler).texture);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			Load(GetComponent<Renderer>().material, "_MainTex", System.IO.Path.Combine(Application.streamingAssetsPath, "plasma.png"));
	}
}