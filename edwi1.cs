using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

public class edwi1 : MonoBehaviour
{
	public string link= "Set website link:";

	IEnumerator process_images(string input) 
	{
		string pattern = @"<img.*?src=""(?<url>.*?)"".*?>";
		Regex re = new Regex(pattern);
		string[] temp_link = link.Split('/');
		foreach(Match m in re.Matches(input))
		{
			string baseword=temp_link[0]+"//"+temp_link[2]+"/"+m.Groups["url"].Value;
			WWW image_file = new WWW(baseword);
			yield return image_file;
			string[] in_dir = m.Groups["url"].Value.Split('/');
			if (in_dir.Length==1)
			{
				File.WriteAllBytes ("C:\\edwi1\\"+m.Groups["url"].Value, image_file.bytes);
			}
			if (in_dir.Length==2)
			{
				Directory.CreateDirectory("C:\\edwi1\\"+in_dir[0]);
				File.WriteAllBytes ("C:\\edwi1\\"+in_dir[0]+"\\"+in_dir[1], image_file.bytes);
			}
		}
	}	

	string konwersja (string zawartosc)
	{
		byte[] bytes = Encoding.Default.GetBytes(zawartosc);
		return  Encoding.UTF8.GetString(bytes);
	}
	
	string process_html(string input)
	{
		string temp=Regex.Replace(input, "<.*?>", "");
		temp= Regex.Replace(temp, @"[^\w\s]", "");
		return temp.ToLower();
	}

	IEnumerator download_website(string url)
	{
		Directory.CreateDirectory("C:\\edwi1\\");
		WWW www = new WWW(url);
		yield return www;		
		Encoding kod = System.Text.Encoding.GetEncoding("windows-1250");
		string bla = kod.GetString(www.bytes);
		System.IO.File.WriteAllText("C:\\edwi1\\edwi1.html", bla);
		StartCoroutine(process_images(www.text));
		string html = File.ReadAllText("C:\\edwi1\\edwi1.html");
		System.IO.File.WriteAllText("C:\\edwi1\\edwi1.txt", process_html(html));
	}

	void OnGUI() 
	{
		link = GUI.TextField(new Rect(10, 10, 400, 20), link, 100);
		if (GUI.Button(new Rect(10, 70, 50, 50), "OK")) StartCoroutine(download_website(link));
	}
}
