using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Text;
using UnityEditor;

public class UnityEnjin : MonoBehaviour
{
	private string _ProjectID = "";
	private string _SecretKey = "";
	private string _Wallet = "";
	private GUIStyle _GUIStyle = new GUIStyle();

	void OnGUI()
	{
		_GUIStyle.fontSize = 32;
		GUI.Label(new Rect(10, 10, 200, 20), "Project ID: ", _GUIStyle);
		_ProjectID = GUI.TextField(new Rect(310, 20, 350, 20), _ProjectID, 25);
		GUI.Label(new Rect(10, 55, 200, 20), "Project Secret Key: ", _GUIStyle);
		_SecretKey = GUI.PasswordField(new Rect(310, 65, 350, 20), _SecretKey, "*"[0], 256);
		GUI.Label(new Rect(10, 100, 200, 20), "Wallet: ", _GUIStyle);
		_Wallet = GUI.PasswordField(new Rect(310, 110, 350, 20), _Wallet, "*"[0], 256);
		if (GUI.Button(new Rect(280, 180, 180, 40), "Get Enjin Wallet Info")) GetResult();
	}

	async void GetResult()
	{
		string result = await GetRequest("query {EnjinWallet(ethAddress: \"" + _Wallet + "\") {enjBalance, tokensCreated {id}}}");
		Debug.Log(result);
	}

	async Task<string> GetAccessToken(string id, string secret)
	{
		Query query = new Query {query = "query {AuthApp(id: " + id + ", secret: \"" + secret + "\") {accessToken expiresIn}}"};
		string jsonData = JsonUtility.ToJson(query);
		UnityWebRequest request = UnityWebRequest.Post("https://kovan.cloud.enjin.io/graphql", UnityWebRequest.kHttpVerbPOST);
		request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData)) as UploadHandler;
		request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
		request.downloadHandler = new DownloadHandlerBuffer();
		await request.SendWebRequest();
		Response response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
		return response.data.AuthApp.accessToken;
	}

	async Task<string> GetRequest(string command)
	{
		string token = await GetAccessToken(_ProjectID, _SecretKey);
		Query query = new Query {query = command};
		string jsonData = JsonUtility.ToJson(query); 
		UnityWebRequest request = UnityWebRequest.Post("https://kovan.cloud.enjin.io/graphql", UnityWebRequest.kHttpVerbPOST);
		request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData)) as UploadHandler;
		request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
		request.SetRequestHeader("Authorization", "Bearer " + token);
		request.downloadHandler = new DownloadHandlerBuffer();
		await request.SendWebRequest();
		return request.downloadHandler.text;
	}
}

public class UnityWebRequestAwaiter : INotifyCompletion
{
	UnityWebRequestAsyncOperation _Request;
	Action _Action; 

	public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation request)
	{
		this._Request = request;
		request.completed += OnRequestCompleted;
	}

	public bool IsCompleted { get { return _Request.isDone; } }

	public UnityWebRequestAsyncOperation GetResult() { return this._Request; }

	public void OnCompleted(Action action)
	{
		this._Action = action;
	}

	private void OnRequestCompleted(AsyncOperation command)
	{
		_Action();
	}
}

public static class UnityWebRequestAsync
{
	public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation request)
	{
		return new UnityWebRequestAwaiter(request);
	}
}

[System.Serializable]
public class Query
{
	public string query;
}

[System.Serializable]
public class Response
{
	public Data data;
}

[System.Serializable]
public class Data
{
	public AuthApp AuthApp;
}

[System.Serializable]
public class AuthApp
{
	public string accessToken;
	public int expiresIn;
}