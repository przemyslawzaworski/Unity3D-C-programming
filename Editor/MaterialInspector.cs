using UnityEngine;
using UnityEditor;

public class MaterialInspector : ShaderGUI
{
	private Material target;
	private MaterialEditor editor;
	private MaterialProperty[] properties;

	public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
	{
		target = (Material)editor.target;
		this.editor = editor;
		this.properties = properties;
		MainMaps();
	}

	void SetKeyword(string keyword, bool state)
	{
		if (state)
			target.EnableKeyword(keyword);
		else
			target.DisableKeyword(keyword);
	}

	GUIContent GenerateLabel(string label, string tooltip)
	{
		GUIContent newLabel = new GUIContent(label);
		newLabel.tooltip = tooltip;
		return newLabel;
	}

	MaterialProperty FindProperty(string name)
	{
		return FindProperty(name, properties);
	}

	void SetTexture(string property, string name)
	{
		MaterialProperty map = FindProperty(property);
		GUIContent label = GenerateLabel(name, name);
		editor.TexturePropertySingleLine(label, map);
	}

	void SetTextureExtra(string property, string name, string extraProperty)
	{
		MaterialProperty map = FindProperty(property);
		GUIContent label = GenerateLabel(name, name);
		editor.TexturePropertySingleLine(label, map, FindProperty(extraProperty));
	}

	void MainMaps()
	{
		GUILayout.Label("Main Maps", EditorStyles.boldLabel);
		SetTextureExtra("_MainTex", "Albedo Map", "_Color");
		SetTexture("_BumpMap", "Normal Map");
		SetTexture("_MetallicGlossMap", "Metallic Map");
		MaterialProperty checkbox = FindProperty("_ShowSlider");
		GUIContent checkboxlabel = GenerateLabel("Show Slider", "Show Slider");
		editor.ShaderProperty(checkbox, checkboxlabel);		
		float show = target.GetFloat("_ShowSlider");
		if (show == 1.0f)
		{
			MaterialProperty slider = FindProperty("_Slider");
			GUIContent sliderlabel = GenerateLabel("Slider", "Slider");
			editor.ShaderProperty(slider, sliderlabel);
		}
	}
}

/*
Shader "Metallic"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo Map", 2D) = "black" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_MetallicGlossMap ("Metallic (R) Smoothness(A) Map", 2D) = "black" {}
		[Toggle] _ShowSlider ("Show Slider", Float) = 1.0
		_Slider ("Slider", Range (0.0, 1.0)) = 0.5
	}

	Subshader
	{
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface SurfaceShader Standard fullforwardshadows addshadow

		sampler2D _MainTex, _BumpMap, _MetallicGlossMap;
		float4 _Color;
		float _ShowSlider, _Slider;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_MetallicGlossMap;
		};

		void SurfaceShader (Input IN, inout SurfaceOutputStandard o) 
		{
			o.Albedo = tex2D(_MainTex,IN.uv_MainTex) * _Color ; 
			o.Normal = UnpackNormal (tex2D(_BumpMap, IN.uv_BumpMap)); 
			o.Metallic = tex2D(_MetallicGlossMap, IN.uv_MetallicGlossMap).r; 
			o.Smoothness = tex2D(_MetallicGlossMap, IN.uv_MetallicGlossMap).a; 
		}

		ENDCG
	}
	
	Fallback "Diffuse"
	CustomEditor "MaterialInspector"	
}
*/