using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

public class GenerateReports : EditorWindow
{
	[MenuItem("Assets/Generate Reports")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow (typeof(GenerateReports));
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Show Shader Keywords" ) ) ShowShaderKeywords();
	} 

	void ShowShaderKeywords ()
	{
		string filePath = Path.Combine(Path.GetTempPath(), "report.html");
		StreamWriter writer = new StreamWriter(filePath);
		string header = "<html><head><style>table{border-collapse: collapse;} td{border: 2px solid #000000;}</style></head><body>";
		string footer = "</body></html>";
		writer.WriteLine(header);
		string[] guids = AssetDatabase.FindAssets("t:shader", null);
		writer.WriteLine("<table>");
		BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.NonPublic;
		int count = 1;
		for (int i = 0; i < guids.Length; i++)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
			Shader shader = AssetDatabase.LoadAssetAtPath<Shader>( assetPath );
			if (shader != null)
			{
				MethodInfo methodInfo = typeof(ShaderUtil).GetMethod("GetShaderGlobalKeywords", bindingAttr);
				string[] keywords = (string[]) methodInfo.Invoke(null, new object[]{ shader });
				for (int j = 0; j < keywords.Length; j++) 
				{
					writer.WriteLine("<tr>");
					writer.WriteLine("<td>" + count + "</td>");
					writer.WriteLine("<td>" + shader.name + "</td>");
					writer.WriteLine("<td>" + keywords[j] + "</td>");
					writer.WriteLine("</tr>");
					count++;
				}
			}
		}
		writer.WriteLine("</table>");
		writer.WriteLine(footer);
		writer.Close();
		System.Diagnostics.Process.Start(filePath);
	}
}