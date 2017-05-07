using UnityEngine;
using System.Collections;

public class quit_and_cursoroff : MonoBehaviour 
{
	void Start ()
	{
		Cursor.visible = false;
	}

	void Update ()
	{
		if (Input.GetKeyDown("escape")) Application.Quit();
	}
}
