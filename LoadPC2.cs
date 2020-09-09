// Example how to import and visualise tree wind animation. May be useful for custom tree rendering.
// Open Speedtree Modeler, import lowpoly tree model, Export Mesh (as FBX), select Wind, set Cache Options Format to PC2.
// Then set filepath (line 22). Play.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LoadPC2 : MonoBehaviour
{
	GameObject[] _Cubes;
	int _Frame = 0;
	int _NumPoints;
	int _NumSamples;
	Vector3[,] _Positions;
	int _FramesPerSecond = 30;

	void Start()
	{
		BinaryReader reader = new BinaryReader(File.OpenRead("E:\\FBX\\WhiteBirch_Low.pc2"));
		byte[] signature = reader.ReadBytes(11);
		if (System.Text.Encoding.UTF8.GetString(signature, 0, signature.Length) != "POINTCACHE2") 
		{
			Debug.Log("Input PC2 file is incorrect!");
			reader.Close();
			return;
		}
		reader.BaseStream.Seek(12, SeekOrigin.Begin);
		int version = reader.ReadInt32();
		_NumPoints = reader.ReadInt32();
		float startSample = reader.ReadSingle();
		float sampleRate = reader.ReadSingle();
		_NumSamples = reader.ReadInt32();
		_Cubes = new GameObject[_NumPoints];
		for (int i = 0; i < _NumPoints; i++)
		{
			_Cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			_Cubes[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			_Cubes[i].hideFlags = HideFlags.HideInHierarchy;
		}
		_Positions = new Vector3[_NumSamples, _NumPoints];
		for (int j = 0; j < _NumSamples; j++)
		{
			for (int i = 0; i < _NumPoints; i++)
			{
				float x = reader.ReadSingle();
				float y = reader.ReadSingle();
				float z = reader.ReadSingle();
				_Positions[j, i] = new Vector3(x, y, z);
			}
		}
		reader.Close();
		InvokeRepeating("WindAnimation", 0.0f, 1.0f / (float)_FramesPerSecond);
	}

	void WindAnimation()
	{
		if (_Frame >= _NumSamples) _Frame = 0;
		for (int i = 0; i < _NumPoints; i++) _Cubes[i].transform.position = _Positions[_Frame, i];
		_Frame++;
	}
}