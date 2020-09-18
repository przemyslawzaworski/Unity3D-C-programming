using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;

public class LoadFont : MonoBehaviour
{
	private GUIStyle _Style, _NewStyle;
	private string _FontName;
	
//// Method nr 1 ///////////////////////////////////////////////////////////////////////////////////////////////	
	[DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] 
	static extern bool GetFontResourceInfoW(string lpszFilename, [In, Out] ref int cbBuffer, [Out] StringBuilder lpBuffer, int dwQueryType);

	string GetFontNameFromWinApi(string fileName) 
	{
		int bufferSize = 0;
		StringBuilder sb = new StringBuilder();
		if (!GetFontResourceInfoW(fileName, ref bufferSize, sb, 1)) {Debug.LogError("GetFontName failed");}
		sb.Capacity = bufferSize / sizeof(char);
		if (!GetFontResourceInfoW(fileName, ref bufferSize, sb, 1)) {Debug.LogError("GetFontName failed");}
		return sb.ToString();
	}

//// Method nr 2 ///////////////////////////////////////////////////////////////////////////////////////////////	
	string GetFontNameFromRegistry(string fileName) 
	{
		RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\Fonts", false);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (key != null)
		{
			string[] keys = key.GetValueNames();
			for (int i = 0; i < keys.Length; i++) dictionary.Add(key.GetValue(keys[i]).ToString(), keys[i].ToString());
			key.Close();
		}
		if (dictionary.TryGetValue(fileName, out string description))
		{
			return description.Substring(0, description.IndexOf("(") - 1);
		}
		return "Error";
	}

//// Method nr 3 ///////////////////////////////////////////////////////////////////////////////////////////////	
	/* With Visual Studio, make .NET Framework DLL (Release / x64);
	using System.Drawing.Text;

	namespace FontInfo
	{
		public class FontInfo
		{
			public string GetFontName(string filePath)
			{
				PrivateFontCollection collection = new PrivateFontCollection();
				collection.AddFontFile(filePath);
				return collection.Families[0].Name;
			}
		}
	}
	*/

	string GetFontNameFromManagedDLL(string dllPath, string fontPath)
	{
		Assembly assembly = Assembly.LoadFrom(dllPath);
		Type type = assembly.GetType("FontInfo.FontInfo");
		dynamic instance = Activator.CreateInstance(type);
		return instance.GetFontName(fontPath);
	}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////	
	bool IsArrayContains (string[] array, string element)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Equals(element)) return true;
		}
		return false;
	}

	void Start()
	{
		string name1 = GetFontNameFromWinApi("C:\\WINDOWS\\Fonts\\ACaslonPro-Bold.otf");
		string name2 = GetFontNameFromRegistry("ACaslonPro-Bold.otf");
		string name3 = GetFontNameFromManagedDLL("E:\\DLL\\FontInfo.dll", "C:\\WINDOWS\\Fonts\\ACaslonPro-Bold.otf");
		string[] fonts = Font.GetOSInstalledFontNames();
		bool result1 = IsArrayContains(fonts, name1);
		bool result2 = IsArrayContains(fonts, name2);
		bool result3 = IsArrayContains(fonts, name3);
		Debug.Log("WinApi: " + name1 + ": " + result1);
		Debug.Log("Registry: " + name2 + ": " + result2);
		Debug.Log("System.Drawing.Text: " + name3 + ": " + result3);
		_FontName = result1 ? name1 : result2 ? name2 : result3 ? name3 : "Arial";
		_Style = new GUIStyle();
		_Style.font = Font.CreateDynamicFontFromOSFont("Courier New", 36);
		_Style.fontSize	= 36;
		_NewStyle = new GUIStyle();
		_NewStyle.font = Font.CreateDynamicFontFromOSFont(_FontName, 36);
		_NewStyle.fontSize	= 36;
	}

	void OnGUI()
	{
		if (Input.GetKey(KeyCode.Space))
			GUI.Label(new Rect(10, 10, 100, 60), _NewStyle.font.name, _NewStyle);
		else
			GUI.Label(new Rect(10, 10, 100, 60), _Style.font.name, _Style);
	}
}