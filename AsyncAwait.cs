using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AsyncAwait : MonoBehaviour
{
	uint _ArrayLength = 8192u;
	bool _Cancel = false;

	async Task GenerateElement(uint index)
	{
		if (_Cancel) 
		{
			await Task.CompletedTask;
		}
		else
		{
			float x = 0.0f;
			for (uint i = 0u; i < (1024u * 1024u); i++)
			{
				x = x + Mathf.Sqrt(i);
			}
			float percent = (float)(index + 1u) / (float)(_ArrayLength) * 100.0f;
			Debug.Log(percent.ToString("N2") + " % ");
			await Task.Delay(1);
		}
	}

	async Task GenerateArray()
	{
		for (uint i = 0; i < _ArrayLength; i++)
		{
			await GenerateElement(i);
		}
	}

	async void Start()
	{
		await GenerateArray();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			_Cancel = true;
	}

	void OnDisable()
	{
		_Cancel = true;
	}
}