using UnityEngine;
using System.Collections;

public class invisible_object : MonoBehaviour 
{
	void Start () 
	{
		Renderer renderer = GetComponent<Renderer>();
		renderer.enabled = false;
	}	
}
