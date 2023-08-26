using UnityEngine;

public class Displacement : MonoBehaviour
{
	public Transform Sphere;

	private Vector3 _StartPosition = Vector3.zero;
	private GUIStyle _GUIStyle = new GUIStyle();
	private float _Displacement = 0f;
	private float _Time = 0f;

	float Function(float x)
	{
		return 0.5f * Mathf.Sin(x) * x;
	}

	void Start()
	{
		_StartPosition = Sphere.position;
		_GUIStyle.fontSize = 32;
		_GUIStyle.normal.textColor = Color.white;
	}

	void Update()
	{
		_Time = Time.time;
		_Displacement = Function(_Time);
		Sphere.position = _StartPosition + new Vector3(_Displacement, 0f, 0f);
	}

	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 100, 20), "Time: " + _Time.ToString(), _GUIStyle);
		GUI.Label(new Rect(10, 80, 100, 20), "Displacement: " + _Displacement.ToString(), _GUIStyle);
	}
}