using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class WindowsForms
{
	[DllImport("user32.dll", SetLastError = true, CharSet= CharSet.Auto)]
	public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

	public static void MessageBox(String text, String caption)
	{
		MessageBox(IntPtr.Zero, text, caption, 0);
	}
}