using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class PlayMusic : MonoBehaviour
{
	public AudioSource AudioSrc;
	
	private string[] _FilePaths;
	private uint _CurrentIndex = 0;
	private AudioClip _AudioClip;
	
    void Start()
    {
        _FilePaths = Directory.GetFiles(Application.streamingAssetsPath, "*.wav");
		StartCoroutine(GetAudioClip(AudioSrc, _FilePaths[_CurrentIndex]));
    }

    IEnumerator GetAudioClip(AudioSource audioSource, string filePath)
    {
		string url = string.Format("file://{0}", filePath);
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                _AudioClip = DownloadHandlerAudioClip.GetContent(www);
				audioSource.clip = _AudioClip;
				audioSource.Play();
            }
        }
    }
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			if (_CurrentIndex < (_FilePaths.Length - 1))
				_CurrentIndex++;
			else
				_CurrentIndex = 0;
			Destroy(_AudioClip);
			StartCoroutine(GetAudioClip(AudioSrc, _FilePaths[_CurrentIndex]));
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			if (_CurrentIndex > 0)
				_CurrentIndex--;
			else
				_CurrentIndex = (uint)_FilePaths.Length - 1u;
			Destroy(_AudioClip);	
			StartCoroutine(GetAudioClip(AudioSrc, _FilePaths[_CurrentIndex]));			
		}
	}
}
