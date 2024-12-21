using UnityEngine;

public class SoftwareRasterizer : MonoBehaviour
{
	Texture2D _ScreenBuffer;
	Color[] _Pixels;
	int _ScreenWidth, _ScreenHeight;
	Matrix4x4 _ModelViewProjection;

	readonly Vector3[] _Vertices = 
	{
		// Cube vertices (12 triangles)
		new Vector3(-1, -1,  1), new Vector3( 1, -1,  1), new Vector3( 1,  1,  1),
		new Vector3(-1, -1,  1), new Vector3( 1,  1,  1), new Vector3(-1,  1,  1),
		new Vector3(-1, -1, -1), new Vector3(-1,  1, -1), new Vector3( 1,  1, -1),
		new Vector3(-1, -1, -1), new Vector3( 1,  1, -1), new Vector3( 1, -1, -1),
		new Vector3(-1, -1, -1), new Vector3(-1, -1,  1), new Vector3(-1,  1,  1),
		new Vector3(-1, -1, -1), new Vector3(-1,  1,  1), new Vector3(-1,  1, -1),
		new Vector3( 1, -1, -1), new Vector3( 1,  1, -1), new Vector3( 1,  1,  1),
		new Vector3( 1, -1, -1), new Vector3( 1,  1,  1), new Vector3( 1, -1,  1),
		new Vector3(-1,  1, -1), new Vector3(-1,  1,  1), new Vector3( 1,  1,  1),
		new Vector3(-1,  1, -1), new Vector3( 1,  1,  1), new Vector3( 1,  1, -1),
		new Vector3(-1, -1, -1), new Vector3( 1, -1, -1), new Vector3( 1, -1,  1),
		new Vector3(-1, -1, -1), new Vector3( 1, -1,  1), new Vector3(-1, -1,  1),
	};

	void Init()
	{
		_ScreenWidth = Screen.width;
		_ScreenHeight = Screen.height;
		_ScreenBuffer = new Texture2D(_ScreenWidth, _ScreenHeight, TextureFormat.RGBA32, false);
		_Pixels = new Color[_ScreenWidth * _ScreenHeight];
		ClearScreen();
	}

	Vector4 UnityObjectToClipPos(Vector3 position)
	{
		return _ModelViewProjection * new Vector4(position.x, position.y, position.z, 1);
	}

	void ClearScreen()
	{
		for (int i = 0; i < _Pixels.Length; i++)
		{
			_Pixels[i] = Color.black;
		}
	}

	Vector2 ProjectToScreen(Vector3 vertex)
	{
		Vector4 clipPos = VertexShader (vertex);
		clipPos /= clipPos.w;
		return new Vector2((clipPos.x + 1.0f) * 0.5f * _ScreenWidth, (clipPos.y + 1.0f) * 0.5f * _ScreenHeight);
	}

	void RasterizeTriangle(Vector2 a, Vector2 b, Vector2 c, Color colorA, Color colorB, Color colorC)
	{
		int minX = Mathf.Clamp(Mathf.Min((int)a.x, (int)b.x, (int)c.x), 0, _ScreenWidth - 1);
		int maxX = Mathf.Clamp(Mathf.Max((int)a.x, (int)b.x, (int)c.x), 0, _ScreenWidth - 1);
		int minY = Mathf.Clamp(Mathf.Min((int)a.y, (int)b.y, (int)c.y), 0, _ScreenHeight - 1);
		int maxY = Mathf.Clamp(Mathf.Max((int)a.y, (int)b.y, (int)c.y), 0, _ScreenHeight - 1);
		for (int y = minY; y <= maxY; y++)
		{
			for (int x = minX; x <= maxX; x++)
			{
				Vector2 p = new Vector2(x, y);
				float w0 = EdgeFunction(b, c, p);
				float w1 = EdgeFunction(c, a, p);
				float w2 = EdgeFunction(a, b, p);
				float area = EdgeFunction(a, b, c);
				if (area > 0 && w0 >= 0 && w1 >= 0 && w2 >= 0)
				{
					w0 /= area;
					w1 /= area;
					w2 /= area;
					int index = y * _ScreenWidth + x;
					if (index >= 0 && index < _Pixels.Length)
					{
						Color color = w0 * colorA + w1 * colorB + w2 * colorC;
						_Pixels[index] = FragmentShader(color);
					}
				}
			}
		}
	}

	float EdgeFunction(Vector2 a, Vector2 b, Vector2 c)
	{
		return (c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x);
	}
	
	Vector4 VertexShader(Vector3 position)
	{
		return UnityObjectToClipPos(position);
	}
	
	Color FragmentShader(Color color)
	{
		return color;
	}	
	
	void Start()
	{
		Init();
	}	
	
	void Update()
	{
		if (_ScreenWidth != Screen.width || _ScreenHeight != Screen.height)
		{
			Init();
		}
		ClearScreen();
		_ModelViewProjection = Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix;
		for (int i = 0; i < _Vertices.Length; i += 3)
		{
			Vector2 screenPosA = ProjectToScreen(_Vertices[i + 0]);
			Vector2 screenPosB = ProjectToScreen(_Vertices[i + 1]);
			Vector2 screenPosC = ProjectToScreen(_Vertices[i + 2]);
			RasterizeTriangle(screenPosA, screenPosB, screenPosC, Color.red, Color.green, Color.blue);
		}
		_ScreenBuffer.SetPixels(_Pixels);
		_ScreenBuffer.Apply();
	}	

	void OnGUI()
	{
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _ScreenBuffer, ScaleMode.StretchToFill, true);
	}
}