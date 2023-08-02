using UnityEngine;

public class Integrals : MonoBehaviour
{
	Material _Red, _White;

	float Function (float x)
	{
		return 0.5f * (Mathf.Sin(x) + x * Mathf.Cos(x)); // integral = 0.5f * Mathf.Sin(x) * x
	}

	// Indefinite integrals (antiderivatives); 
	// Depends on input function, results can have value offset (Constant of Integration);
	float Integral (float x, int n) 
	{
		float stepSize = x / n;
		float integral = 0f;
		for (int i = 0; i < n; i++)
		{
			float x0 = i * stepSize;
			float x1 = x0 + stepSize;
			float y0 = Function(x0);
			float y1 = Function(x1);
			integral += 0.5f * stepSize * (y0 + y1);
		}
		return integral;
	}

	void Start()
	{
		_Red = new Material(Shader.Find("Legacy Shaders/Diffuse"));
		_Red.color = Color.red;
		_White = new Material(Shader.Find("Legacy Shaders/Diffuse"));
		_White.color = Color.white;
		float stepSize = 0.1f;
		for (float x = -16f; x < 16f; x += stepSize)
		{
			GameObject red = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			red.transform.position = new Vector3(x, 0f, 0.5f * Mathf.Sin(x) * x);  // check
			red.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			red.GetComponent<Renderer>().sharedMaterial = _Red;
			GameObject white = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			white.transform.position = new Vector3(x, 0f, Integral(x, 100));
			white.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			white.GetComponent<Renderer>().sharedMaterial = _White;
		}
	}

	void OnDestroy()
	{
		Destroy(_Red);
		Destroy(_White);
	}
}