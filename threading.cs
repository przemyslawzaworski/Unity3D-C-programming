// Author: Przemyslaw Zaworski
// Example of using multithreading to accelerate point vs bounding box intersection tests.
using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

public class threading : MonoBehaviour
{
	public int Elements = 256 * 256;
	public int Threads = 8;
	public bool Render = false;

	private Vector3[] _Centers, _Scales;
	private Task[] _Tasks;
	private bool[] _Results;

	Vector3 GetRandomVector(Vector3 min, Vector3 max)
	{
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			byte[] data = new byte[4];
			rng.GetBytes(data);
			rng.Dispose();
			float x = System.Convert.ToSingle(System.BitConverter.ToInt32(data, 0));
			float a = System.Convert.ToSingle(System.Int32.MinValue);
			float b = System.Convert.ToSingle(System.Int32.MaxValue);
			vector[i] = (x - a) / (b - a) * (max[i] - min[i]) + min[i];
		}
		return vector;
	}

	float Box (Vector3 p, Vector3 c, Vector3 s)
	{
		float mx = Mathf.Max(p[0] - c[0] - s[0], c[0] - p[0] - s[0]);
		float my = Mathf.Max(p[1] - c[1] - s[1], c[1] - p[1] - s[1]);   
		float mz = Mathf.Max(p[2] - c[2] - s[2], c[2] - p[2] - s[2]);
		return Mathf.Max(Mathf.Max(mx, my), mz);
	}

	bool WorkerThread (Vector3 point, int threadId, int threadDim, int arraySize)
	{
		int start = threadId * arraySize / threadDim;
		int end = (threadId + 1) * arraySize / threadDim;
		for (int i = start; i < end; i++)
		{
			float distance = Box (point, _Centers[i], _Scales[i]);
			if (distance < 0.0f) return true;
		}
		return false;
	}

	bool Execute (Vector3 point, int threadDim, int arraySize)
	{
		for (int i = 0; i < _Tasks.Length; i++) 
		{
			int threadId = i; // We have to use temporary here !
			_Tasks[i] = Task.Run(new Action(() => _Results[threadId] = WorkerThread(point, threadId, threadDim, arraySize)));
		}
		Task.WaitAll(_Tasks);
		return Array.Exists(_Results, element => element == true);
	}

	void Start()
	{
		Threads = (Threads > 0) ? Threads : SystemInfo.processorCount;
		_Centers = new Vector3[Elements];
		_Scales = new Vector3[Elements];
		_Tasks = new Task[Threads];
		_Results = new bool[Threads];
		for (int i = 0; i < Elements; i++)
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.hideFlags = HideFlags.HideInHierarchy;
			cube.transform.position = GetRandomVector(new Vector3(10f, 10f, 10f), new Vector3(50f, 50f, 50f));
			cube.transform.localScale = GetRandomVector(new Vector3(1f, 1f, 1f), new Vector3(3f, 3f, 3f));
			_Centers[i] = cube.transform.position;
			_Scales[i] = cube.transform.localScale * 0.5f;
			cube.GetComponent<Renderer>().enabled = Render;
		}
	}

	void Update () 
	{
		bool hit = Execute (Camera.main.transform.position, Threads, Elements);
		if (hit) Debug.Log("Intersection = TRUE");
	}
}