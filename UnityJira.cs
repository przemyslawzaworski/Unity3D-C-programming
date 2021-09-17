using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Text;
using System.Linq;
using System.IO;

public class UnityJira : MonoBehaviour
{
	public string Url = "https://company.atlassian.net"; //no slash or backslash on the end
	public string User = "user@company.com";
	public string Token = ""; // here insert private token generated from: https://id.atlassian.com/manage-profile/security/api-tokens
	public string ProjectKey = "ABC";
	public string IssuePriority = "Medium";
	public string IssueCategory = "Task";
	public bool AttachOutputLog = true;

	private string _Credentials;
	private string _Summary = "Summary";
	private string _Description = "Description";
	private List<IssuePreference> _IssuePriorities;
	private List<EntryType> _IssueTypes;

	[Serializable]
	public class Entry
	{
		public string id;
		public string key;
		public string self;
	}

	[Serializable]
	class Priority
	{	
		public string self;
		public string statusColor;
		public string description;
		public string iconUrl;
		public string name;
		public int id;
	}

	[Serializable]
	public class AvatarUrls
	{
		public string _48x48;
		public string _24x24;
		public string _16x16;
		public string _32x32;
	}

	[Serializable]
	public class Lead
	{
		public string self;
		public string accountId;
		public AvatarUrls avatarUrls;
		public string displayName;
		public bool active;
	}

	[Serializable]
	public class IssueType
	{
		public string self;
		public string id;
		public string description;
		public string iconUrl;
		public string name;
		public bool subtask;
		public int avatarId;
		public int hierarchyLevel;
	}

	[Serializable]
	public class Version
	{
		public string self;
		public string id;
		public string description;
		public string name;
		public bool archived;
		public bool released;
		public string releaseDate;
		public string userReleaseDate;
		public int projectId;
		public string startDate;
		public string userStartDate;
	}

	[Serializable]
	public class Roles
	{
		public string AdministratorsMigrated;
		public string AtlassianAddonsProjectAccess;
		public string Administrators;
	}

	[Serializable]
	public class Properties {}

	[Serializable]
	public class Project
	{
		public string expand;
		public string self;
		public string id;
		public string key;
		public string description;
		public Lead lead;
		public List<object> components;
		public List<IssueType> issueTypes;
		public string assigneeType;
		public List<Version> versions;
		public string name;
		public Roles roles;
		public AvatarUrls avatarUrls;
		public string projectTypeKey;
		public bool simplified;
		public string style;
		public bool isPrivate;
		public Properties properties;
	}

	[System.Serializable]
	public class EntryType 
	{
		public int id;
		[System.NonSerialized] public string name;
	}

	[System.Serializable]
	public class IssuePreference 
	{
		public string id;
		[System.NonSerialized] public string name;
	}

	[System.Serializable]
	public class Issue 
	{
		public EntryKey project;
		public string summary;
		public string description;
		public EntryType issuetype;
		public IssuePreference priority;
	}

	[System.Serializable]
	public class EntryKey 
	{
		public string key;
	}	

	public static class JsonHelper
	{
		public static T[] FromJson<T>(string json)
		{
			Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
			return wrapper.Items;
		}

		[Serializable]
		private class Wrapper<T>
		{
			public T[] Items;
		}
	}

	IEnumerator Start()
	{
		string credentials = User + ":" + Token;
		byte[] bytes = System.Text.Encoding.UTF8.GetBytes(credentials);
		_Credentials = System.Convert.ToBase64String(bytes);
		yield return GetProjectIssueTypes(_Credentials);
		yield return GetProjectPriorities(_Credentials);
	}

	string CombinePaths(params string[] paths)
	{
		if (paths == null)
		{
			throw new ArgumentNullException("paths");
		}
		return paths.Aggregate(System.IO.Path.Combine);
	}

	byte[] GetBytesFromFilePath(string pathToOutputLog) // to avoid IOException: Sharing violation
	{
		using (var fileStream = File.Open(pathToOutputLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) 
		{
			using (var memoryStream = new MemoryStream())
			{
				fileStream.CopyTo(memoryStream);
				return memoryStream.ToArray();
			}
		}
	}

	IEnumerator GetProjectIssueTypes(string credentials)
	{
		string url = Url + "/rest/api/2/project" + "/" + ProjectKey;
		UnityWebRequest request = new UnityWebRequest(url, "GET");
		request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		request.SetRequestHeader("Authorization", "Basic " + credentials);
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.isNetworkError || request.isHttpError) 
		{
			Debug.Log("Error: " + request.responseCode + " - " + request.downloadHandler.text);
		}
		else
		{
			Project project = JsonUtility.FromJson<Project>(request.downloadHandler.text);
			_IssueTypes = new List<EntryType>();
			for (int i = 0; i < project.issueTypes.Count; i++)
			{
				if (project.issueTypes[i].subtask == false)
				{
					EntryType issueType = new EntryType();
					issueType.name = project.issueTypes[i].name;
					issueType.id = int.Parse(project.issueTypes[i].id);
					_IssueTypes.Add(issueType);
				}
			}
		}
	}

	IEnumerator GetProjectPriorities(string credentials) 
	{
		string url = Url + "/rest/api/2/priority";
		UnityWebRequest request = new UnityWebRequest(url, "GET");
		request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		request.SetRequestHeader("Authorization", "Basic " + credentials);
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.isNetworkError || request.isHttpError) 
		{
			Debug.Log("Error: " + request.responseCode + " - " + request.downloadHandler.text);
		}
		else
		{
			string json = "{\"Items\":" + request.downloadHandler.text + "}";
			Priority[] priorities = JsonHelper.FromJson<Priority>(json);
			_IssuePriorities = new List<IssuePreference>();
			for (int i = 0; i < priorities.Length; i++) 
			{
				IssuePreference preference = new IssuePreference();
				preference.name = priorities[i].name;
				preference.id = priorities[i].id.ToString();
				_IssuePriorities.Add(preference);
			}
		}
	}

	public IEnumerator CreateIssue(string summary, string description, string credentials) 
	{
		Issue issue = new Issue();
		issue.summary = summary;
		issue.description = description;
		for (int i = 0; i < _IssueTypes.Count; i++) 
		{
			if (_IssueTypes[i].name == IssueCategory) 
			{
				EntryType entryType = new EntryType();
				entryType.id = _IssueTypes[i].id;
				issue.issuetype = entryType;
				break;
			}
		}
		for (int i = 0; i < _IssuePriorities.Count; i++)
		{
			if (_IssuePriorities[i].name == IssuePriority) 
			{
				IssuePreference preference = new IssuePreference();
				preference.id = _IssuePriorities[i].id;
				issue.priority = preference;
				break;
			}
		}
		EntryKey entryKey = new EntryKey();
		entryKey.key = ProjectKey;
		issue.project = entryKey;
		string json = "{\"fields\":" + JsonUtility.ToJson(issue).ToString() + "}";
		byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
		UnityWebRequest request = new UnityWebRequest(Url + "/rest/api/2/issue", "POST");
		request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
		request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		request.SetRequestHeader("Authorization", "Basic " + credentials);
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.isNetworkError || request.isHttpError) 
		{
			Debug.Log("Error: " + request.responseCode + " - " + request.downloadHandler.text);
		} 
		else 
		{
			Debug.Log("Jira Issue created!");
			Entry entry = JsonUtility.FromJson<Entry>(request.downloadHandler.text);
			string path = "";
			// path = Path.Combine(Application.persistentDataPath, "output_log.txt");
#if (!UNITY_EDITOR && UNITY_STANDALONE_WIN)
			path = CombinePaths(Environment.GetEnvironmentVariable("AppData"), "..", "LocalLow", Application.companyName, Application.productName, "output_log.txt");
#endif
#if (UNITY_EDITOR && UNITY_EDITOR_WIN)
			path = CombinePaths(Environment.GetEnvironmentVariable("AppData"), "..", "Local", "Unity", "Editor", "Editor.log");
#endif
			if (File.Exists(path) && AttachOutputLog)
			{
				byte[] logData = GetBytesFromFilePath(path);
				string logName = "output_log.txt";
				WWWForm form = new WWWForm();
				form.AddBinaryData("file", logData, logName);
				UnityWebRequest attachment = UnityWebRequest.Post(Url + "/rest/api/2/issue" + "/" + entry.key + "/attachments", form);
				attachment.SetRequestHeader("X-Atlassian-Token", "no-check");
				attachment.SetRequestHeader("Authorization", "Basic " + credentials);
				attachment.SendWebRequest();
				if (attachment.isNetworkError || attachment.isHttpError) 
				{
					Debug.Log("Error " + attachment.responseCode + " - " + attachment.downloadHandler.text);
				} 
				else 
				{
					Debug.Log("Output Log attach completed!");
				}
			}
		}
	}

	void OnGUI()
	{
		_Summary = GUI.TextField(new Rect(10, 10, 800, 50), _Summary);
		_Description = GUI.TextField(new Rect(10, 80, 800, 200), _Description);
		if (GUI.Button(new Rect(365, 320, 70, 50), "OK")) StartCoroutine(CreateIssue(_Summary, _Description, _Credentials));		
	}
}
