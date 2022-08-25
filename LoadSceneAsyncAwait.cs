using System.Collections;
using System;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAsyncAwait : MonoBehaviour
{
	async Task LoadSceneTask (string sceneName)
	{
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
		asyncOperation.allowSceneActivation = false;
		while (asyncOperation.progress < 0.9f) { }
		asyncOperation.allowSceneActivation = true;
	}

	async void LoadScene (string sceneName)
	{
		await LoadSceneTask (sceneName);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			LoadScene ("Menu");
		}
	}
}