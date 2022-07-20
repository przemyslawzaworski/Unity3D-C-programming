using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class LoadCSV : MonoBehaviour
{
	List<List<string>> LoadToLists (string filePath, char[] separator) // load CSV to list of lists
	{
		List<List<string>> lists = new List<List<string>>();
		if (!File.Exists(filePath)) return lists;
		StreamReader reader = new StreamReader(filePath);
		StringSplitOptions stringSplitOptions = StringSplitOptions.RemoveEmptyEntries;
		while (true)
		{
			string line = reader.ReadLine();
			if (line == null) break;
			List<string> cells = line.Split(separator, stringSplitOptions).ToList();
			lists.Add(cells);
		}
		reader.Close();
		return lists;
	}

	string[][] LoadToArrays (string filePath, char[] separator) // load CSV to array of arrays
	{
		List<List<string>> lists = new List<List<string>>();
		if (!File.Exists(filePath)) return new string[0][];
		StreamReader reader = new StreamReader(filePath);
		StringSplitOptions stringSplitOptions = StringSplitOptions.RemoveEmptyEntries;
		while (true)
		{
			string line = reader.ReadLine();
			if (line == null) break;
			List<string> cells = line.Split(separator, stringSplitOptions).ToList();
			lists.Add(cells);
		}
		reader.Close();
		return lists.Select(x => x.ToArray()).ToArray();
	}

	string[,] LoadToArray2D (string filePath, char[] separator) // load CSV to 2D array
	{
		List<List<string>> lists = new List<List<string>>();
		if (!File.Exists(filePath)) return new string[0,0];
		StreamReader reader = new StreamReader(filePath);
		StringSplitOptions stringSplitOptions = StringSplitOptions.RemoveEmptyEntries;
		int length = 0;
		while (true)
		{
			string line = reader.ReadLine();
			if (line == null) break;
			List<string> cells = line.Split(separator, stringSplitOptions).ToList();
			length = Mathf.Max(length, cells.Count);
			lists.Add(cells);
		}
		reader.Close();
		string[,] array = new string[lists.Count, length];
		for (int y = 0; y < lists.Count; y++)
			for (int x = 0; x < lists[y].Count; x++)
				array[y, x] = lists[y][x];
		return array;
	}

	void Start() // test
	{
		string filePath = Path.Combine(Application.streamingAssetsPath, "test.csv");
		List<List<string>> lists = LoadToLists(filePath, new char[] {','});
		for (int y = 0; y < lists.Count; y++)
			for (int x = 0; x < lists[y].Count; x++)
				Debug.Log(lists[y][x]);
		Debug.Log("*****************************************");
		string[][] arrays = LoadToArrays(filePath, new char[] {','});
		for (int y = 0; y < arrays.Length; y++)
			for (int x = 0; x < arrays[y].Length; x++)
				Debug.Log(arrays[y][x]);
		Debug.Log("*****************************************");
		string[,] array2D = LoadToArray2D(filePath, new char[] {','});
		for (int y = 0; y < array2D.GetLength(0); y++)
			for (int x = 0; x < array2D.GetLength(1); x++)
				Debug.Log(array2D[y,x]);
	}
}