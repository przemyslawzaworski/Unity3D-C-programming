// Rearrange randomly an array.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shuffle : MonoBehaviour 
{	
	void Shuffle(Vector4[] elements)
	{
		for (int i = 0; i < elements.Length; i++ )
		{
			Vector4 tmp = elements[i];
			int r = Random.Range(i, elements.Length);
			elements[i] = elements[r];
			elements[r] = tmp;
		}
	}
	
	void Start () 
	{
		Vector4[] vectors = new Vector4[5];	
		vectors[0] = new Vector4(34f,31f,46f,73f);
		vectors[1] = new Vector4(95f,9f,29f,55f);
		vectors[2] = new Vector4(10f,88f,85f,15f);
		vectors[3] = new Vector4(35f,42f,74f,64f);
		vectors[4] = new Vector4(86f,8f,32f,89f);
		Shuffle(vectors);
		for (int k=0;k<vectors.Length;k++) Debug.Log(vectors[k]);
	}
	
}
