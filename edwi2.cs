using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class edwi2 : MonoBehaviour
{
	public string link= "Set website link:";

	public static bool IsNullOrWhiteSpace(string value)
	{
		if (value != null)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (!char.IsWhiteSpace(value[i])) return false;
			}
		}
		return true;
	}

	string process_html(string input)
	{
		string temp=Regex.Replace(input, "<.*?>", "");
		temp= Regex.Replace(temp, @"[^\w\s]", "");
		return temp.ToLower();
	}

	IEnumerator download_website(string url)
	{
		Directory.CreateDirectory("C:\\edwi2\\");
		WWW www = new WWW(url);
		yield return www;
		File.WriteAllText("C:\\edwi2\\edwi2.html", www.text);
		string html = File.ReadAllText("C:\\edwi2\\edwi2.html");
		File.WriteAllText("C:\\edwi2\\edwi2.txt", process_html(html));
		sorting();
	}
	
	void sorting ()
	{
		string source = File.ReadAllText("C:\\edwi2\\edwi2.txt");
		string[] words = source.Split(' ');
		words = words.Where((s) => { return (0 != String.Compare(s, "")); }).ToArray();
		words = words.Where(arg => !IsNullOrWhiteSpace(arg)).ToArray();
		Array.Sort(words);
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		int num=1;
		foreach (string key in words)
		{
			if (!dictionary.ContainsKey(key)) 
			{
				num=1;
				dictionary.Add(key,num);
			}
			else
			{
				num++;
				dictionary[key] = num;
			}
		}
		var result = from pair in dictionary orderby pair.Value descending select pair;
		List<string> output = new List<string>();
		foreach (KeyValuePair<string, int> pair in result)
		{
			output.Add(pair.Key.Trim()+" "+pair.Value);
		}
		File.WriteAllLines("C:\\edwi2\\sorting.txt", output.ToArray());
		Debug.Log("Sortowanie zakonczone!");
	}

	void OnGUI() 
	{
		link = GUI.TextField(new Rect(10, 10, 400, 20), link, 100);
		if (GUI.Button(new Rect(10, 70, 50, 50), "OK"))
		{
			StartCoroutine(download_website(link));
		}
	}
}