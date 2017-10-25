//2D Rotation in the direction of target.
//Author: Przemyslaw Zaworski
using UnityEngine;
using System.Collections;

public class direction : MonoBehaviour 
{
	public GameObject target;
	
	void Update () 
	{
		Vector2 source = new Vector2(transform.position.x,transform.position.y);
		Vector2 destination = new Vector2(target.transform.position.x,target.transform.position.y);
		float delta = Mathf.Atan ((destination.x - source.x) / (destination.y - source.y));
		float angle = delta * Mathf.Rad2Deg;
		if (((destination.x - source.x) > 0.0f) && ((destination.y - source.y) < 0.0f)) angle = angle + 180.0f;
		if (((destination.x - source.x) < 0.0f) && ((destination.y - source.y) < 0.0f)) angle = angle + 180.0f;
		if (((destination.x - source.x) < 0.0f) && ((destination.y - source.y) > 0.0f)) angle = angle + 360.0f;
		transform.rotation = Quaternion.Euler(0, 0, -angle);	
	}
}
