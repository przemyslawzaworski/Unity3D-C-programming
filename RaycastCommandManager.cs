using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class RaycastCommandManager
{
	private int _Count = 0;
	private int _Commands = 32;
	private NativeArray<Vector3> _Positions;
	private NativeArray<Vector3> _Directions;
	private NativeArray<float> _Distances;
	private NativeArray<int> _Layers;
	private RaycastHit[] _RaycastHits;

	struct PrepareRaycastCommands : IJobParallelFor
	{
		public NativeArray<RaycastCommand> Raycasts;
		[ReadOnly] public NativeArray<Vector3> Positions;
		[ReadOnly] public NativeArray<Vector3> Directions;
		[ReadOnly] public NativeArray<float> Distances;
		[ReadOnly] public NativeArray<int> Layers;
		public void Execute(int i)
		{
			Raycasts[i] = new RaycastCommand(Positions[i], Directions[i], Distances[i], Layers[i]);
		}
	}

	public RaycastCommandManager(List<Vector3> positions, List<Vector3> directions, List<float> distances, List<int> layers, int commands = 32)
	{
		_Count = positions.Count;
		_Commands = commands;
		_RaycastHits = new RaycastHit[_Count];
		_Positions = new NativeArray<Vector3>(_Count, Allocator.Persistent);
		_Directions = new NativeArray<Vector3>(_Count, Allocator.Persistent);
		_Distances = new NativeArray<float>(_Count, Allocator.Persistent);
		_Layers = new NativeArray<int>(_Count, Allocator.Persistent);
		for (int i = 0; i < _Count; i++)
		{
			_Positions[i] = positions[i];
			_Directions[i] = directions[i];
			_Distances[i] = distances[i];
			_Layers[i] = layers[i];
		}
	}

	public RaycastHit[] Update()
	{
		NativeArray<RaycastCommand> raycastCommands = new NativeArray<RaycastCommand>(_Count, Allocator.TempJob);
		NativeArray<RaycastHit> raycastHits = new NativeArray<RaycastHit>(_Count, Allocator.TempJob);
		PrepareRaycastCommands setupRaycastsJob = new PrepareRaycastCommands()
		{
			Raycasts = raycastCommands,
			Positions = _Positions,
			Directions = _Directions,
			Distances = _Distances,
			Layers = _Layers
		};
		JobHandle setupDependency = setupRaycastsJob.Schedule(_Count, _Commands);
		JobHandle raycastDependency = RaycastCommand.ScheduleBatch(raycastCommands, raycastHits, _Commands, setupDependency);
		raycastDependency.Complete();
		raycastHits.CopyTo(_RaycastHits);
		raycastCommands.Dispose();
		raycastHits.Dispose();
		return _RaycastHits;
	}

	public void Release()
	{
		_Positions.Dispose();
		_Directions.Dispose(); 
		_Distances.Dispose();
		_Layers.Dispose();
	}
}