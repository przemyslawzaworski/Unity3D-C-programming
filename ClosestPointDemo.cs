using Unity.Collections;
using Unity.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class ClosestPoint
{
	private Matrix4x4 _LocalToWorldMatrix;
	private NativeArray<Vector3> _Vertices;
	private NativeArray<int> _Triangles;
	private NativeArray<float> _Distances;
	private NativeArray<Vector3> _ClosestPoints;	
	private int _Count = 0;

	struct WorkerThread : IJobParallelFor
	{
		[ReadOnly] public Vector3 Point;
		[ReadOnly] public Matrix4x4 LocalToWorldMatrix;
		[ReadOnly] public NativeArray<Vector3> Vertices;
		[ReadOnly] public NativeArray<int> Triangles;
		public NativeArray<float> Distances;
		public NativeArray<Vector3> ClosestPoints;
		public void Execute(int index)
		{
			Vector3 v0 = LocalToWorldMatrix.MultiplyPoint3x4(Vertices[Triangles[index * 3 + 0]]);
			Vector3 v1 = LocalToWorldMatrix.MultiplyPoint3x4(Vertices[Triangles[index * 3 + 1]]);
			Vector3 v2 = LocalToWorldMatrix.MultiplyPoint3x4(Vertices[Triangles[index * 3 + 2]]);
			Vector3 closestPoint = ClosestPointOnTriangleToPoint(Point, v0, v1, v2);
			Distances[index] = Vector3.Distance(Point, closestPoint);
			ClosestPoints[index] = closestPoint;
		}
	}
	
	static Vector3 ClosestPointOnTriangleToPoint(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
	{
		Vector3 ba = b - a;
		Vector3 pa = p - a;
		Vector3 cb = c - b;
		Vector3 pb = p - b;
		Vector3 ac = a - c;
		Vector3 pc = p - c;
		Vector3 nm = Vector3.Cross(ba, ac);
		Vector3 q = Vector3.Cross(nm, pa);
		float d = 1.0f / Vector3.Dot(nm, nm);
		float u = d * Vector3.Dot(q, ac);
		float v = d * Vector3.Dot(q, ba);
		float w = 1.0f - u - v;       
		if (u < 0.0f)
		{
			w = Mathf.Clamp(Vector3.Dot(pc, ac) / Vector3.Dot(ac, ac), 0.0f, 1.0f);
			u = 0.0f;
			v = 1.0f - w;
		}
		else if (v < 0.0f)
		{
			u = Mathf.Clamp(Vector3.Dot(pa, ba) / Vector3.Dot(ba, ba), 0.0f, 1.0f);
			v = 0.0f;
			w = 1.0f - u;
		}
		else if (w < 0.0f)
		{
			v = Mathf.Clamp(Vector3.Dot(pb, cb) / Vector3.Dot(cb, cb), 0.0f, 1.0f);
			w = 0.0f;
			u = 1.0f - v;
		}
		return u * b + v * c + w * a;
	}	

	public ClosestPoint(MeshCollider meshCollider)
	{
		Vector3[] vertices = meshCollider.sharedMesh.vertices;
		int[] triangles = meshCollider.sharedMesh.triangles;
		_Count = triangles.Length / 3;
		_Vertices = new NativeArray<Vector3>(vertices.Length, Allocator.Persistent);
		_Triangles = new NativeArray<int>(triangles.Length, Allocator.Persistent);
		_Distances = new NativeArray<float>(_Count, Allocator.Persistent);
		_ClosestPoints = new NativeArray<Vector3>(_Count, Allocator.Persistent);
		unsafe
		{
			fixed (void* source = vertices)
			{
				void* destination = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(_Vertices);
				UnsafeUtility.MemCpy(destination, source, vertices.Length * UnsafeUtility.SizeOf<Vector3>());
			}
		}
		unsafe
		{
			fixed (void* source = triangles)
			{
				void* destination = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(_Triangles);
				UnsafeUtility.MemCpy(destination, source, triangles.Length * UnsafeUtility.SizeOf<int>());
			}
		}		
	}

	public Vector3 Update(MeshCollider meshCollider, Vector3 worldPos)
	{
		_LocalToWorldMatrix = meshCollider.transform.localToWorldMatrix;		
		WorkerThread workerThread = new WorkerThread()
		{
			Point = worldPos,
			LocalToWorldMatrix = _LocalToWorldMatrix,
			Vertices = _Vertices,
			Distances = _Distances,
			Triangles = _Triangles,
			ClosestPoints = _ClosestPoints
		};
		JobHandle jobHandle = workerThread.Schedule(_Count, 1);
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
		return _ClosestPoints[index];
	}

	public void Release()
	{
		_Distances.Dispose();
		_Vertices.Dispose();
		_Triangles.Dispose();
		_ClosestPoints.Dispose();
	}
}

public class ClosestPointDemo : MonoBehaviour
{
	[SerializeField] MeshCollider _MeshCollider;
	[SerializeField] Transform _Point;	
	ClosestPoint _ClosestPoint;

	void Start()
	{
		_ClosestPoint = new ClosestPoint(_MeshCollider);
	}

	void Update()
	{
		Vector3 closestPoint = _ClosestPoint.Update(_MeshCollider, _Point.position);
		Debug.DrawLine(_Point.position, closestPoint, Color.blue);
	}

	void OnDestroy()
	{
		_ClosestPoint.Release();
	}
}