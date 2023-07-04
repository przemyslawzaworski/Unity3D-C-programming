using UnityEngine;
using UnityEditor;

public class Scatterplot : EditorWindow
{
	[MenuItem("Assets/Scatter Plot")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow (typeof(Scatterplot));
	}

	bool active = true;

	static string source =
	@"
	Shader ""Hidden/Scatterplot""
	{
		SubShader
		{
			Pass
			{
				CGPROGRAM
				#pragma vertex VSMain
				#pragma fragment PSMain
				#pragma target 5.0

				sampler2D _RenderTexture;
				float2 _Center;
				
				float Circle (float2 p, float2 c, float r)
				{
					return step(length(p - c) - r, 0.0);
				}

				float4 VSMain (float4 vertex:POSITION, inout float2 uv:TEXCOORD0) : SV_POSITION
				{
					return UnityObjectToClipPos(vertex);
				}

				float4 PSMain (float4 vertex : SV_POSITION, float2 uv : TEXCOORD0) : SV_TARGET
				{
					float a = tex2D(_RenderTexture, uv).r;
					float b = Circle(uv, _Center, 0.005) * 0.1;
					return float4((a + b).xxx, 1.0);
				}
				ENDCG
			}
		}
	}
	"; 

	void OnGUI()
	{
		active = GUILayout.Toggle(active, "Show Only Enabled Components");
		if (GUILayout.Button("Generate Plot")) GeneratePlot();
	}

	void GeneratePlot()
	{
		MeshRenderer[] meshRenderers = FindObjectsOfType<MeshRenderer>(true);
		Vector4 corners = GetWorldSpaceCorners(meshRenderers);
		Debug.Log((corners.z - corners.x).ToString() + " x " + (corners.w - corners.y).ToString());
		RenderTexture rta = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32);
		RenderTexture rtb = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32);
		Shader shader = ShaderUtil.CreateShaderAsset(source, false);
		Material material = new Material(shader);
		for (int i = 0; i < meshRenderers.Length; i++)
		{
			if (meshRenderers[i].enabled == false && active) continue;
			Vector3 position = meshRenderers[i].bounds.center; 
			float x = Mathf.InverseLerp(corners.x, corners.z, position.x);
			float y = Mathf.InverseLerp(corners.y, corners.w, position.z);
			material.SetVector("_Center", new Vector4(x, y, 0f, 0f));
			bool swap = i % 2 == 0;
			material.SetTexture("_RenderTexture", swap ? rta : rtb);
			RenderToTexture(swap ? rta : rtb, swap ? rtb : rta, material);
		}
		SaveRenderTextureToFile(rta);
		DestroyImmediate(material);
		DestroyImmediate(shader);
		rta.Release();
		rtb.Release();
	}

	Vector4 GetWorldSpaceCorners(MeshRenderer[] meshRenderers)
	{
		float minX, minZ, maxX, maxZ;
		minX = minZ = System.Single.MaxValue;
		maxX = maxZ = System.Single.MinValue;
		for (int i = 0; i < meshRenderers.Length; i++)
		{
			if (meshRenderers[i].enabled == false && active) continue;
			Vector3 position = meshRenderers[i].bounds.center;
			minX = Mathf.Min(minX, position.x);
			minZ = Mathf.Min(minZ, position.z);
			maxX = Mathf.Max(maxX, position.x);
			maxZ = Mathf.Max(maxZ, position.z);
		}
		return new Vector4(minX, minZ, maxX, maxZ);
	}

	void SaveRenderTextureToFile (RenderTexture rt)
	{
		RenderTexture.active = rt;
		Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
		texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
		RenderTexture.active = null;
		byte[] bytes = texture.EncodeToPNG();  
		string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Scatterplot.png");
		System.IO.File.WriteAllBytes(filePath, bytes); 
		System.Diagnostics.Process.Start(filePath);
	}

	void RenderToTexture (RenderTexture source, RenderTexture destination, Material material)
	{
		RenderTexture.active = destination;
		GL.PushMatrix();
		GL.LoadOrtho();
		GL.invertCulling = true;
		material.SetPass(0);
		GL.Begin(GL.QUADS);
		GL.MultiTexCoord2(0, 0.0f, 0.0f);
		GL.Vertex3(0.0f, 0.0f, 0.0f);
		GL.MultiTexCoord2(0, 1.0f, 0.0f);
		GL.Vertex3(1.0f, 0.0f, 0.0f); 
		GL.MultiTexCoord2(0, 1.0f, 1.0f);
		GL.Vertex3(1.0f, 1.0f, 0.0f); 
		GL.MultiTexCoord2(0, 0.0f, 1.0f);
		GL.Vertex3(0.0f, 1.0f, 0.0f);
		GL.End();
		GL.invertCulling = false;
		GL.PopMatrix();
	}
}