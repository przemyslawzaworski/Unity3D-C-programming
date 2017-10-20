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
	public string k = "10";
	public string t = "4";

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

	IEnumerator download_website(string url, int K, int T)
	{
		Directory.CreateDirectory("C:\\edwi2\\");
		WWW www = new WWW(url);
		yield return www;
		File.WriteAllText("C:\\edwi2\\edwi2.html", www.text);
		string html = File.ReadAllText("C:\\edwi2\\edwi2.html");
		File.WriteAllText("C:\\edwi2\\edwi2.txt", process_html(html));
		sorting(K,T);
	}
	
	void sorting (int K, int T)
	{
		string source = File.ReadAllText("C:\\edwi2\\edwi2.txt");
		string[] words = source.Split(' ');
		words = words.Where((s) => { return (0 != String.Compare(s, "")); }).ToArray();
		words = words.Where(arg => !IsNullOrWhiteSpace(arg)).ToArray();
		float poczatek = Time.realtimeSinceStartup; 
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
		int processed = 0;
		foreach (KeyValuePair<string, int> pair in result)
		{
			if (pair.Value < T) break;
			output.Add(pair.Key.Trim()+" "+pair.Value);
			if (++processed == K) break;
		}
		float koniec = Time.realtimeSinceStartup - poczatek;
		File.WriteAllLines("C:\\edwi2\\sorting.txt", output.ToArray());
		Debug.Log("Sortowanie zakonczone! Czas trwania to: "+String.Format( "{0:0.000000}",koniec)+"  sekund.");
	}

	void OnGUI() 
	{
		link = GUI.TextField(new Rect(10, 10, 400, 20), link, 100);
		k = GUI.TextField(new Rect(420, 10, 60, 20), k, 10);
		t = GUI.TextField(new Rect(500, 10, 60, 20), t, 10);
		int K = 0;
		Int32.TryParse(k, out K);
		int T = 0;
		Int32.TryParse(t, out T);
		if (GUI.Button(new Rect(10, 70, 50, 50), "OK"))
		{
			StartCoroutine(download_website(link,K,T));
		}
	}
}