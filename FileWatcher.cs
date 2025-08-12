using System;
using System.IO;
using System.Collections.Concurrent;
using UnityEngine;

public class FileWatcher : MonoBehaviour
{
	private FileSystemWatcher fileSystemWatcher;
	private readonly ConcurrentQueue<string> concurrentQueue = new ConcurrentQueue<string>();

	void Start()
	{
		string fullPath = Path.GetFullPath(Application.persistentDataPath);
		Debug.Log(fullPath);
		if (!Directory.Exists(fullPath))
		{
			Debug.LogError($"Directory not exists: {fullPath}");
			return;
		}
		fileSystemWatcher = new FileSystemWatcher(fullPath, "*.txt");
		fileSystemWatcher.IncludeSubdirectories = true;
		fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
		fileSystemWatcher.Changed += OnChanged;
		fileSystemWatcher.Created += OnChanged;
		fileSystemWatcher.Renamed += OnRenamed;
		fileSystemWatcher.Deleted += OnDeleted;
		fileSystemWatcher.EnableRaisingEvents = true;
	}

	void OnChanged(object sender, FileSystemEventArgs e)
	{
		concurrentQueue.Enqueue($"Changed file: {e.FullPath}");
	}

	void OnDeleted(object sender, FileSystemEventArgs e)
	{
		concurrentQueue.Enqueue($"Deleted file: {e.FullPath}");
	}

	void OnRenamed(object sender, RenamedEventArgs e)
	{
		concurrentQueue.Enqueue($"Renamed file {e.OldFullPath} to {e.FullPath}");
	}

	void Update()
	{
		while (concurrentQueue.TryDequeue(out string change))
		{
			Debug.Log(change);
		}
	}

	void OnDestroy()
	{
		if (fileSystemWatcher != null)
		{
			fileSystemWatcher.EnableRaisingEvents = false;
			fileSystemWatcher.Dispose();
		}
	}
}