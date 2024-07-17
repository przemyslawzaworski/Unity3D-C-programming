using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example how to use MaterialPropertyBlock
public class MaterialPropertyBlock : MonoBehaviour
{
	[SerializeField] MeshRenderer _MeshRenderer;
	UnityEngine.MaterialPropertyBlock _MaterialPropertyBlock;
	const string _ColorPropertyName = "_Color";

	void Start()
	{
		_MaterialPropertyBlock = new UnityEngine.MaterialPropertyBlock();
		_MeshRenderer.GetPropertyBlock(_MaterialPropertyBlock);
	}

	void Update()
	{
		_MaterialPropertyBlock.SetColor(_ColorPropertyName, Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f));
		_MeshRenderer.SetPropertyBlock(_MaterialPropertyBlock);
	}
}