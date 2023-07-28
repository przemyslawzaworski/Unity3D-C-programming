using UnityEngine;

public class Derivatives : MonoBehaviour
{
	Material _Red, _Blue, _Black;

	float Function (float x)
	{
		return 0.5f * Mathf.Sin(x) * x;  // derivative = 0.5f * (Mathf.Sin(x) + x * Mathf.Cos(x))
	}

	float Derivative (float x, float h)
	{
		return (Function(x + h) - Function(x - h)) / (2f * h);
	}

	void Start()
	{
		_Red = new Material(Shader.Find("Legacy Shaders/Diffuse"));
		_Red.color = Color.red;
		_Blue = new Material(Shader.Find("Legacy Shaders/Diffuse"));
		_Blue.color = Color.blue;
		_Black = new Material(Shader.Find("Legacy Shaders/Diffuse"));
		_Black.color = Color.black;
		float stepSize = 0.1f;
		for (float x = -16f; x < 16f; x += stepSize)
		{
			GameObject red = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			red.transform.position = new Vector3(x, 0f, Function(x));
			red.GetComponent<Renderer>().sharedMaterial = _Red;
		//////////////////////////////////////////////////////////////////////////
			GameObject blue = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			blue.transform.position = new Vector3(x, 0f, Derivative(x, stepSize));
			blue.GetComponent<Renderer>().sharedMaterial = _Blue;
		//////////////////////////////////////////////////////////////////////////
			GameObject black = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			black.transform.position = new Vector3(x, 0f, 0.5f * (Mathf.Sin(x) + x * Mathf.Cos(x))); // check
			black.GetComponent<Renderer>().sharedMaterial = _Black;
		}
	}

	void OnDestroy()
	{
		Destroy(_Red);
		Destroy(_Blue);
		Destroy(_Black);
	}
}