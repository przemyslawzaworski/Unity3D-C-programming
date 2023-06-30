using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Display : MonoBehaviour
{
	async Task MoveWindowTask (int index) // Minimum Unity Version required: 2021.2
	{
		List<DisplayInfo> displayLayout = new List<DisplayInfo>();
		Screen.GetDisplayLayout(displayLayout);
		if (index < displayLayout.Count)
		{
			DisplayInfo display = displayLayout[index]; 
			Vector2Int position = new Vector2Int(0, 0);
			if (Screen.fullScreenMode != FullScreenMode.Windowed)
			{
				position.x += display.width / 2;
				position.y += display.height / 2;
			}
			AsyncOperation asyncOperation = Screen.MoveMainWindowTo(display, position);
			while (asyncOperation.progress < 1f) 
			{
				await Task.Yield();
			}
		}
		else
		{
			await Task.CompletedTask;
		}
	}

	async void MoveWindowAsync (int index)
	{
		await MoveWindowTask (index);
	}

	void Update()
	{
		for (KeyCode keyCode = KeyCode.Keypad0; keyCode <= KeyCode.Keypad9; keyCode++)
		{
			if (Input.GetKeyDown(keyCode))
			{
				string key = keyCode.ToString();
				int index = System.Convert.ToInt32(key[key.Length-1].ToString());
				MoveWindowAsync(index);
				break;
			}
		}
	}
}