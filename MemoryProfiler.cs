// For best results, use in Windows Standalone (development build).

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Profiling;
using System.Linq;
using System.IO;

public class MemoryProfiler : MonoBehaviour
{
	public KeyCode FirstMeasurementKey = KeyCode.J;
	public KeyCode SecondMeasurementKey = KeyCode.K;
	public KeyCode ShowDifferenceKey = KeyCode.L;
	public string ClassName = "UnityEngine.Material";

	void Update()
	{
		MemoryProfilerUpdate();
	}

	void MemoryProfilerUpdate()
	{
		if (Input.GetKeyDown(FirstMeasurementKey))
		{
			DumpObjectInfo("first.txt", ClassName);	
		}
		if (Input.GetKeyDown(SecondMeasurementKey))
		{	
			DumpObjectInfo("second.txt", ClassName);
		}
		if (Input.GetKeyDown(ShowDifferenceKey))
		{	
			ShowDifferenceInfo("first.txt", "second.txt");
		}
	}
	
	// Shows all objects with selected type from memory (object name### (GetInstanceID) : object memory size in bytes)
	void DumpObjectInfo (string filename, string typedef)
	{
		Type type = Type.GetType(typedef + ",UnityEngine.dll");
		UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll(type);
		string result = "";
		Dictionary<string, long> dictionary = new Dictionary<string, long>();
		for (int i = 0; i < objects.Length; i++)
		{
			dictionary.Add(objects[i].name + "### (" + objects[i].GetInstanceID() + ")", Profiler.GetRuntimeMemorySizeLong(objects[i]));
		}	
		IEnumerable rows = from pair in dictionary orderby pair.Value descending select pair;
		foreach (KeyValuePair<string, long> pair in rows)
		{
			result = result + pair.Key.ToString() + " : " +  pair.Value.ToString() + "\n";
		}			
		string path = System.IO.Path.GetTempPath() + "\\" + filename;
		System.IO.File.WriteAllText(path, result, System.Text.Encoding.UTF8);
		if (File.Exists("C:\\Program Files (x86)\\Notepad++\\notepad++.exe")) 
			System.Diagnostics.Process.Start("C:\\Program Files (x86)\\Notepad++\\notepad++.exe", path);
		else
			System.Diagnostics.Process.Start("notepad.exe", path);
	}

	// Shows entries which are in filename2 and not in filename1.
	void ShowDifferenceInfo (string filename1, string filename2)
	{
		string path1 = System.IO.Path.GetTempPath() + "\\" + filename1;
		string path2 = System.IO.Path.GetTempPath() + "\\" + filename2;
		if (!File.Exists(path1) || !File.Exists(path2)) return;
		string result = "";
		string line = "";
		string[] split = new string[0];
		List<string> firstResults = new List<string>();
		StreamReader reader = new StreamReader(path1);
		while (true)
		{
			line = reader.ReadLine();
			if (line == null) break;
			split = line.Split(new string[] { "###" }, StringSplitOptions.None);
			firstResults.Add(split[0]);
		}
		reader.Close();			
		reader = new StreamReader(path2);	
		StreamWriter writer = new StreamWriter(System.IO.Path.GetTempPath() + "\\difference.txt");			
		while (true)
		{
			line = reader.ReadLine();
			if (line == null) break;
			split = line.Split(new string[] { "###" }, StringSplitOptions.None);
			if (!(firstResults.Contains(split[0]))) writer.WriteLine(line);			
		}
		reader.Close();
		writer.Close();
		string path = System.IO.Path.GetTempPath() + "\\difference.txt";
		if (File.Exists(path))
		{
			if (File.Exists("C:\\Program Files (x86)\\Notepad++\\notepad++.exe")) 
				System.Diagnostics.Process.Start("C:\\Program Files (x86)\\Notepad++\\notepad++.exe", path);
			else
				System.Diagnostics.Process.Start("notepad.exe", path);			
		}			
	}
}
