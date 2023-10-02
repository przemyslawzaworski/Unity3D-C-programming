using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class LSystem : MonoBehaviour
{
	private string _Axiom = "F";
	private string _CurrentString;
	private int _Iterations = 4;
	private Dictionary<char, string> _Rules = new Dictionary<char, string>();

	public struct TransformInfo
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public TransformInfo(Vector3 position, Quaternion rotation)
		{
			Position = position;
			Rotation = rotation;
		}
	}

	void Start()
	{
		_Rules['F'] = "FF+[+F-F+F]-[-F+F+F]";
		_CurrentString = _Axiom;
		GenerateLSystem();
		RenderLSystem();
	}

	void GenerateLSystem()
	{
		for (int i = 0; i < _Iterations; i++)
		{
			StringBuilder nextString = new StringBuilder();
			foreach (char c in _CurrentString)
			{
				if (_Rules.ContainsKey(c))
				{
					nextString.Append(_Rules[c]);
				}
				else
				{
					nextString.Append(c);
				}
			}
			_CurrentString = nextString.ToString();
		}
	}

	void RenderLSystem()
	{
		Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
		float length = 3f;
		float angle = 25f;
		List<CombineInstance> instances = new List<CombineInstance>();
		List<GameObject> capsules = new List<GameObject>();
		foreach (char c in _CurrentString)
		{
			if (c == 'F')
			{
				GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
				capsule.transform.localScale = new Vector3(1f, length, 1f);
				capsule.transform.position = transform.position;
				capsule.transform.rotation = transform.rotation;
				transform.position += transform.up * length;
				capsule.transform.parent = this.transform;
				CombineInstance combineInstance = new CombineInstance();
				combineInstance.mesh = capsule.GetComponent<MeshFilter>().sharedMesh;
				combineInstance.transform = capsule.transform.localToWorldMatrix;
				capsule.SetActive(false);
				instances.Add(combineInstance);
				capsules.Add(capsule);
			}
			else if (c == '+')
			{
				transform.Rotate(Vector3.forward * angle);
			}
			else if (c == '-')
			{
				transform.Rotate(Vector3.back * angle);
			}
			else if (c == '[')
			{
				transformStack.Push(new TransformInfo(transform.position, transform.rotation));
			}
			else if (c == ']')
			{
				TransformInfo info = transformStack.Pop();
				transform.position = info.Position;
				transform.rotation = info.Rotation;
			}
		}
		MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.CombineMeshes(instances.ToArray());
		meshFilter.sharedMesh = mesh;
		MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
		meshRenderer.sharedMaterial.SetColor("_Color", Color.green);
		Debug.Log(_CurrentString.Length);
		for (int i = capsules.Count - 1; i > -1; i--) Destroy(capsules[i]);
	}
}