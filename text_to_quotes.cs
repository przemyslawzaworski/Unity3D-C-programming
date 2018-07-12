using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class text_to_quotes : MonoBehaviour 
{
	public string InputPath;
	public string OutputPath;
	
	void Start () 
	{
		StreamReader reader = new StreamReader(InputPath);
		StreamWriter writer = new StreamWriter(OutputPath);
		while (!reader.EndOfStream)
		{
			string line = "\"" + reader.ReadLine() + "\"";
			writer.WriteLine(line);		
		}
		reader.Close();
		writer.Close();
		Debug.Log("Done.");
	}
	
}
