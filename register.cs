using UnityEngine;
using Microsoft.Win32;

public class register : MonoBehaviour 
{
	void Start () 
	{
		RegistryKey handle = Registry.CurrentUser;
		handle = handle.OpenSubKey("Software", true);
		RegistryKey key = handle.CreateSubKey("Game");
		key.SetValue("Fullscreen",1);
		handle.Close();
	}
}
