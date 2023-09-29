using UnityEngine;
using System;

public class Levenshtein : MonoBehaviour
{
	int Distance(string a, string b)
	{
		int[,] m = new int[a.Length + 1, b.Length + 1];
		for (int i = 0; i <= a.Length; i++) m[i, 0] = i;
		for (int j = 0; j <= b.Length; j++) m[0, j] = j;
		for (int i = 1; i <= a.Length; i++)
		{
			for (int j = 1; j <= b.Length; j++)
			{
				int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
				m[i, j] = Math.Min(Math.Min(m[i - 1, j] + 1, m[i, j - 1] + 1), m[i - 1, j - 1] + cost);
			}
		}
		return m[a.Length, b.Length];
	}

	void Start()
	{
		Debug.Log(Distance("word", "swords")); // 2
		Debug.Log(Distance("brake", "break")); // 2
	}
}

/* Levenshtein distance is a string metric for measuring the difference between two sequences. 
Informally, the Levenshtein distance between two words is the minimum number of single-character edits (insertions, deletions or substitutions) 
required to change one word into the other. */