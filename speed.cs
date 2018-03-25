using UnityEngine;

public class speed : MonoBehaviour 
{
	public GameObject item;
	Vector3 old_value;
	float result;
	
	float ComputeSpeed(GameObject game_object)
	{
		float new_value = ((game_object.transform.position - old_value).magnitude) / Time.deltaTime;
		old_value = game_object.transform.position;
		return new_value;
	}

	void Update () 
	{
		result = ComputeSpeed(item);
	}
	
	void OnGUI() 
	{
		GUI.Label(new Rect(10, 40, 200, 100), result.ToString());
	}
}
