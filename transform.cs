using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class transform : MonoBehaviour 
{
	Vector3 StartPosition;
	Vector3 StartScale;
	void Awake()
	{
		StartPosition = transform.position;
		StartScale = new Vector3(1,1,1);
	}
	
	void Update() 
	{
		if (Input.GetKey("a")) transform.position = new Vector3(0, 0, 0); 
		else 
		if (Input.GetKey("b")) transform.Translate(new Vector3(0,0.1f,0));
		else
		if (Input.GetKey("c")) transform.Rotate(new Vector3(0,10,0));	
		else
		if (Input.GetKey("d")) transform.localScale+=(new Vector3(0.1f,0.1f,0.1f) );		
		else 
		{
			transform.position=StartPosition;
			transform.localScale=StartScale;
		}
    }
}