using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
 
public class Plasma : MonoBehaviour
{
	[SerializeField] private int _Resolution = 256;
	private Material _Material = null;
	private Texture2D _Texture = null;

	struct ProceduralTexture : IJobParallelFor
	{
		public NativeArray<Color32> Colors;
		[ReadOnly] public int Resolution;
		[ReadOnly] public float Timer;

		float Fract(float x) { return x - Mathf.Floor(x); }

		public void Execute(int index)
		{
			Vector2 fragCoord = new Vector2(index % Resolution, index / Resolution);
			Vector2 uv = new Vector2(fragCoord.x / Resolution, fragCoord.y / Resolution);
			Vector2 p = new Vector2(uv.x, uv.y) * 2f;
			for (int i = 1; i < 64; i++)
			{
				Vector2 q = new Vector2(p.x, p.y);
				q.x += (0.5f / (2.0f * (float)i)) * Mathf.Cos((float)i * p.y + Timer + 0.05f * ((float)i + 20)) + 1.5f;
				q.y += (0.5f / (2.0f * (float)i)) * Mathf.Cos((float)i * p.x + Timer + 0.05f * ((float)i + 10)) + 2.0f;
				p = new Vector2(q.x, q.y);
			}
			float r = Fract(1.5f * p.x - 0.5f);
			float g = Fract(1.5f * p.y - 0.5f);
			float b = Fract(0.5f * p.x + p.y - 0.5f);
			Colors[index] = new Color32 ((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), 255);
		}
	}

	void Start()
	{
		_Texture = new Texture2D(_Resolution, _Resolution, TextureFormat.RGBA32, false, false);
		_Material = new Material(Shader.Find("Unlit/Texture"));
		_Material.mainTexture = _Texture;
		GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		plane.GetComponent<Renderer>().material = _Material;
	}

	void Update()
	{
		NativeArray<Color32> colors = _Texture.GetRawTextureData<Color32>();
		ProceduralTexture proceduralTexture = new ProceduralTexture()
		{
			Colors = colors,
			Resolution = _Resolution,
			Timer = Time.time
		};
		JobHandle jobHandle = proceduralTexture.Schedule(_Resolution * _Resolution, 1);
		_Texture.Apply(false);
		jobHandle.Complete();
	}

	void OnDestroy()
	{
		Destroy(_Texture);
		Destroy(_Material);
	}
}