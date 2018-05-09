using UnityEngine;

public class parabola : MonoBehaviour 
{
	public GameObject Player;
	public Vector3 StartPosition;
	public Vector3 EndPosition;	
	public float Height;
	public float Speed;	
	private float startTime;
	private bool enable = false;

	Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
	{
		float p = t * 2 - 1;
		Vector3 d = end - start;
		Vector3 s = start + t * d;
		s.y += ( -p * p + 1 ) * height;
		return s;
	}
 
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Space)) 
		{
			enable = true;
			startTime = Time.time;
		}
		if (enable)
		{
			float time = (Time.time - startTime) ;
			if (time*Speed<=1.0f) Player.transform.position = Parabola (StartPosition,EndPosition,Height,time*Speed);
		}
	}
}
