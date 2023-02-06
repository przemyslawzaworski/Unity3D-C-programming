using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using System;
using System.Text;

namespace UnityOpenAI
{
	public class UnityOpenAI : MonoBehaviour
	{
		public string Model = "text-davinci-003"; // read more: https://platform.openai.com/playground
		[Range(0f, 1f)]  public float Temperature = 0.7f; // controls randomness
		[Range(1, 4000)] public int MaximumLength = 2048; // maximum number of tokens to generate
		[Range(0f, 1f)]  public float TopP = 1f; // controls diversity
		[Range(0f, 2f)]  public float FrequencyPenalty = 0f;
		[Range(0f, 2f)]  public float PresencePenalty = 0f;
		public string Key = ""; // https://platform.openai.com/account/api-keys

		string _Input = "Click here, delete text and type new input text...", _Output = "Press Submit and wait for output...";
		GUIStyle _GUIStyle;

		void Start()
		{
			Camera.main.clearFlags = CameraClearFlags.SolidColor;
			Camera.main.backgroundColor = Color.black;
		}

		void OnGUI()
		{
			_GUIStyle = new GUIStyle(GUI.skin.FindStyle("textField"));
			_GUIStyle.wordWrap = true;
			_Input = GUI.TextField(new Rect(0, 0, Screen.width, Screen.height / 2 - 100), _Input, _GUIStyle);
			GUI.Label(new Rect(0, Screen.height / 2 - 100, Screen.width, Screen.height / 2 + 50), _Output, _GUIStyle);
			if (GUI.Button(new Rect(0, Screen.height - 37, Screen.width, 25), "Submit")) Submit();
		}

		async void Submit()
		{
			_Output = "Please wait...";
			_Output = await GetRequest(Model, _Input, Temperature, MaximumLength, TopP, FrequencyPenalty, PresencePenalty);
		}

		async Task<string> GetRequest(string model, string input, float temperature, int length, float topp, float frequency, float presence)
		{
			Query query = new Query();
			query.model = model;
			query.prompt = input;
			query.temperature = temperature;
			query.max_tokens = length;
			query.top_p = topp;
			query.frequency_penalty = frequency;
			query.presence_penalty = presence;
			string jsonData = JsonUtility.ToJson(query);
			UnityWebRequest request = UnityWebRequest.Post("https://api.openai.com/v1/completions", jsonData);
			request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.SetRequestHeader("Authorization", "Bearer " + Key);
			await request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.LogError(request.error);
			}
			Response response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
			return (response.choices != null) ? response.choices[0].text.TrimStart('\n').TrimStart('\n') : "";
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
		public string model;
		public string prompt;
		public float temperature;
		public int max_tokens;
		public float top_p;
		public float frequency_penalty;
		public float presence_penalty;
	}

	[System.Serializable]
	public class Choice
	{
		public string text;
		public int index;
		public object logprobs;
		public string finish_reason;
	}

	[System.Serializable]
	public class Usage
	{
		public int prompt_tokens;
		public int completion_tokens;
		public int total_tokens;
	}

	[System.Serializable]
	public class Response
	{
		public string id;
		public string @object;
		public int created;
		public string model;
		public Choice[] choices;
		public Usage usage;
	}
}