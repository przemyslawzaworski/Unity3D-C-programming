using UnityEngine;
using System;

public class Calculus : MonoBehaviour
{
	enum Mode {Functions, Derivatives, Integrals}

	[SerializeField] Mode _Mode;

	Material _Red, _Green, _Blue;
	GUIStyle _GUIStyle;

	float Displacement(float x) // example function
	{
		return 0.5f * Mathf.Sin(x) * x;
	}

	float Velocity(float x) // first derivative
	{
		return 0.5f * (Mathf.Sin(x) + x * Mathf.Cos(x));
	}

	float Acceleration(float x) // second derivative
	{
		return 0.5f * (2.0f * Mathf.Cos(x) - x * Mathf.Sin(x));
	}

	float Derivative(Func<float, float> function, float x, float h)
	{
		return (function(x + h) - function(x - h)) / (2f * h);
	}

	float Integral(Func<float, float> function, float x, int n)
	{
		float stepSize = x / n;
		float integral = 0f;
		for (int i = 0; i < n; i++)
		{
			float x0 = i * stepSize;
			float x1 = x0 + stepSize;
			float y0 = function(x0);
			float y1 = function(x1);
			integral += 0.5f * stepSize * (y0 + y1);
		}
		return integral;
	}

	void Start()
	{
		Func<float, float> fx = x => 0.5f * Mathf.Sin(x) * x;
		Func<Func<float, float>, float, float, float> fdx = (fx, x, h) => Derivative(fx, x, h);
		Func<Func<float, float>, float, float, float> sdx = (fx, x, h) => Derivative(f => fdx(fx, f, h), x, h);
		Func<float, float> fn = x => 0.5f * (2.0f * Mathf.Cos(x) - x * Mathf.Sin(x));
		Func<Func<float, float>, float, int, float> fi = (fn, x, h) => Integral(fn, x, h);
		Func<Func<float, float>, float, int, float> si = (fn, x, h) => Integral(f => fi(fn, f, h), x, h);
		_Red = new Material(Shader.Find("Unlit/Color"));
		_Red.color = Color.red;
		_Green = new Material(Shader.Find("Unlit/Color"));
		_Green.color = new Color(0f, 0.5f, 0f, 1f);
		_Blue = new Material(Shader.Find("Unlit/Color"));
		_Blue.color = Color.blue;
		_GUIStyle = new GUIStyle();
		_GUIStyle.fontSize = 32;
		_GUIStyle.normal.textColor = Color.white;
		float stepSize = 0.01f;
		for (float x = -16f; x < 16f; x += stepSize)
		{
			GameObject red = GameObject.CreatePrimitive(PrimitiveType.Cube);
			float y1 = _Mode == Mode.Functions ? Displacement(x) : _Mode == Mode.Derivatives ? fx(x) : si(fn, x, 100);
			red.transform.position = new Vector3(x, 0f, y1);
			red.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
			red.GetComponent<Renderer>().sharedMaterial = _Red;
		//////////////////////////////////////////////////////////////////////////
			GameObject green = GameObject.CreatePrimitive(PrimitiveType.Cube);
			float y2 = _Mode == Mode.Functions ? Velocity(x) : _Mode == Mode.Derivatives ? fdx(fx, x, stepSize) : fi(fn, x, 100);
			green.transform.position = new Vector3(x, 0.0005f, y2);
			green.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
			green.GetComponent<Renderer>().sharedMaterial = _Green;
		//////////////////////////////////////////////////////////////////////////
			GameObject blue = GameObject.CreatePrimitive(PrimitiveType.Cube);
			float y3 = _Mode == Mode.Functions ? Acceleration(x) : _Mode == Mode.Derivatives ? sdx(fx, x, stepSize) : fn(x);
			blue.transform.position = new Vector3(x, 0.001f, y3);
			blue.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
			blue.GetComponent<Renderer>().sharedMaterial = _Blue;
		}
	}

	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 200, 20), _Mode.ToString(), _GUIStyle);
	}

	void OnDestroy()
	{
		Destroy(_Red);
		Destroy(_Green);
		Destroy(_Blue);
	}
}