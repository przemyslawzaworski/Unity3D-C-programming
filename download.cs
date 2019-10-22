// Extraction data from www website - UnityWebRequest simple example
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class download : MonoBehaviour
{
	void Start()
	{
		StartCoroutine(ExtractData("https://en.wikipedia.org/wiki/Main_Page"));
	}

	IEnumerator ExtractData (string url)
	{
		var request = UnityWebRequest.Get(url);
		yield return request.SendWebRequest();
		var data = request.downloadHandler.text;
		Regex re = new Regex("<div.*class=.external text[\\s\\S]*?</div>");
		MatchCollection matches = re.Matches(data);
		foreach (Match m in matches)
		{
			Debug.Log(m.Groups[0].Value.ToString());
		}
	}
}