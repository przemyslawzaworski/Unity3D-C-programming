using UnityEngine;

public class SVN : MonoBehaviour
{
	int GetLocalSvnRevisionNumber()
	{
		System.IO.DirectoryInfo directoryInfo = System.IO.Directory.GetParent(Application.dataPath);
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		process.StartInfo = new System.Diagnostics.ProcessStartInfo()
		{
			UseShellExecute = false,
			CreateNoWindow = true,
			WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
			FileName = "cmd.exe",
			Arguments = "/C SubWCRev " + directoryInfo.FullName,
			RedirectStandardError = true,
			RedirectStandardOutput = true
		};
		process.Start();
		string output = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\D*Updated to revision ([0-9]+)\D*");
		System.Text.RegularExpressions.Match match = regex.Match(output);
		int revision = -1;
		if (match.Success)
		{
			revision = int.Parse(match.Groups[1].Value);
		}
		string[] lines = output.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.None);
		for (int i = 0; i < lines.Length; i++)
		{
			if (lines[i].Contains("Mixed revision range"))
			{
				string[] words = lines[i].Split(new string[] { " " }, System.StringSplitOptions.None);
				string[] subwords = words[words.Length - 1].Split(new string[] { ":" }, System.StringSplitOptions.None);
				if (subwords.Length > 0)
				{
					if (int.TryParse(subwords[0], out int number))
					{
						revision = number;
					}
				}
			}
		}
		return revision;
	}

	void Start()
	{
		Debug.Log(GetLocalSvnRevisionNumber());
	}
}
