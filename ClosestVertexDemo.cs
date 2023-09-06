using Unity.Collections;
using Unity.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class ClosestVertex
{
	private Matrix4x4 _LocalToWorldMatrix;
	private NativeArray<Vector3> _Vertices;
	private NativeArray<float> _Distances;

	struct WorkerThread : IJobParallelFor
	{
		[ReadOnly] public Vector3 Point;
		[ReadOnly] public Matrix4x4 LocalToWorldMatrix;
		[ReadOnly] public NativeArray<Vector3> Vertices;
		public NativeArray<float> Distances;
		public void Execute(int index)
		{
			Distances[index] = Vector3.Distance(Point, LocalToWorldMatrix.MultiplyPoint3x4(Vertices[index]));
		}
	}

	public ClosestVertex(MeshCollider meshCollider)
	{
		Vector3[] vertices = meshCollider.sharedMesh.vertices;
		_Vertices = new NativeArray<Vector3>(vertices.Length, Allocator.Persistent);
		_Distances = new NativeArray<float>(vertices.Length, Allocator.Persistent);
		unsafe
		{
			fixed (void* source = vertices)
			{
				void* destination = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(_Vertices);
				UnsafeUtility.MemCpy(destination, source, vertices.Length * UnsafeUtility.SizeOf<Vector3>());
			}
		}
		_LocalToWorldMatrix = meshCollider.transform.localToWorldMatrix;
	}

	public Vector3 Update(Vector3 worldPos)
	{
		WorkerThread workerThread = new WorkerThread()
		{
			Point = worldPos,
			LocalToWorldMatrix = _LocalToWorldMatrix,
			Vertices = _Vertices,
			Distances = _Distances
		};
		JobHandle jobHandle = workerThread.Schedule(_Vertices.Length, 1);
		jobHandle.Complete();
		float minDistance = 1e9f;
		int index = 0;
		for (int i = 0; i < _Distances.Length; i++)
		{
			if (_Distances[i] < minDistance)
			{
				minDistance = _Distances[i];
				index = i;
			}
		}
		return _LocalToWorldMatrix.MultiplyPoint3x4(_Vertices[index]);
	}

	public void Release()
	{
		_Distances.Dispose();
		_Vertices.Dispose();
	}
}

public class ClosestVertexDemo : MonoBehaviour
{
	[SerializeField] MeshCollider _MeshCollider;
	[SerializeField] Transform _Point;	
	ClosestVertex _ClosestVertex;

	void Start()
	{
	   _ClosestVertex = new ClosestVertex(_MeshCollider);
	}

	void Update()
	{
		Vector3 closestVertex = _ClosestVertex.Update(_Point.position);		
		Debug.DrawLine(_Point.position, closestVertex, Color.blue);       
	}

	void OnDestroy()
	{
		_ClosestVertex.Release();
	}
}