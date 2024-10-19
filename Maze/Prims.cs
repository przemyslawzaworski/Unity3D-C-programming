using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Prims : MonoBehaviour
{
	[Tooltip("Grid dimensions must be [odd, odd]")]
	public Vector2Int Dimensions = new Vector2Int(65, 65);

	private bool[,] _Grid = null;

	IList<Vector2Int> CalculateNeighbors(Vector2Int position, bool returnPaths) 
	{
		List<Vector2Int> paths = new List<Vector2Int>();
		List<Vector2Int> walls = new List<Vector2Int>();
		Vector2Int north = new Vector2Int(position.x, position.y - 2);
		Vector2Int south = new Vector2Int(position.x, position.y + 2);
		Vector2Int west = new Vector2Int(position.x - 2, position.y);
		Vector2Int east = new Vector2Int(position.x + 2, position.y);
		if (north.x >= 0 && north.y >= 0 && north.x < Dimensions.x && north.y < Dimensions.y) 
		{
			if (_Grid[north.x, north.y]) paths.Add(north); else walls.Add(north); 
		}
		if (east.x >= 0 && east.y >= 0 && east.x < Dimensions.x && east.y < Dimensions.y) 
		{
			if (_Grid[east.x, east.y]) paths.Add(east); else walls.Add(east);
		}
		if (south.x >= 0 && south.y >= 0 && south.x < Dimensions.x && south.y < Dimensions.y) 
		{
			if (_Grid[south.x, south.y]) paths.Add(south); else walls.Add(south); 
		}
		if (west.x >= 0 && west.y >= 0 && west.x < Dimensions.x && west.y < Dimensions.y) 
		{
			if (_Grid[west.x, west.y]) paths.Add(west); else walls.Add(west);
		}
		return returnPaths ? paths : walls;
	}

	void GenerateMaze()
	{
		GameObject maze = new GameObject(name: "Maze");
		maze.transform.position = Vector3.zero;
		List<CombineInstance> instances = new List<CombineInstance>();
		Vector2Int position = new Vector2Int(1, 1);
		_Grid[position.x, position.y] = true;
		HashSet<Vector2Int> cells = new HashSet<Vector2Int>();
		cells.UnionWith(CalculateNeighbors(position, false));
		while (cells.Count > 0) 
		{
			Vector2Int cell = cells.ElementAt(Random.Range(0, cells.Count));
			IList<Vector2Int> paths = CalculateNeighbors(cell, true);
			if (paths.Count > 0) 
			{
				Vector2Int path = paths[Random.Range(0, paths.Count)];
				int x = (path.x + cell.x) / 2;
				int y = (path.y + cell.y) / 2;
				_Grid[cell.x, cell.y] = true;
				_Grid[x, y] = true;
			}
			cells.UnionWith(CalculateNeighbors(cell, false));
			cells.Remove(cell);
		}
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Mesh mesh = cube.GetComponent<MeshFilter>().sharedMesh;
		for (int x = 0; x < _Grid.GetLength(0); x++) 
		{
			for (int y = 0; y < _Grid.GetLength(1); y++)
			{
				if (_Grid[x, y] == false)
				{
					cube.transform.position = new Vector3(x, 0f, y);
					cube.transform.parent = maze.transform;
					CombineInstance combineInstance = new CombineInstance();
					combineInstance.mesh = mesh;
					combineInstance.transform = cube.transform.localToWorldMatrix;
					instances.Add(combineInstance);
				}
			}
		}
		Destroy(cube);
		MeshFilter meshFilter = maze.AddComponent<MeshFilter>();
		meshFilter.mesh = new Mesh();
		meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		meshFilter.mesh.CombineMeshes(instances.ToArray());
		MeshRenderer meshRenderer = maze.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = new Material(Shader.Find("Legacy Shaders/Diffuse"));
	}

	void Start()
	{
		_Grid = new bool[Dimensions.x, Dimensions.y];
		GenerateMaze();
	}
}

/*
This Unity C# script implements Prim's Algorithm for maze generation, a randomized method for 
constructing a perfect maze, where there is only one path between any two cells. 
The algorithm was invented by Robert Prim in 1957 and later adapted for maze generation. 
It works by growing the maze step-by-step, starting from a random point and expanding through unvisited cells 
while keeping track of potential walls to carve through.

Key Components:
- Dimensions: The size of the grid for the maze, which must be odd numbers for proper maze generation.
- _Grid: A 2D boolean array representing the maze. true represents a path, while false represents a wall.
- CalculateNeighbors: This method calculates the neighboring cells of a given position, distinguishing between 
paths and walls. It returns either a list of valid paths or a list of walls around the given position.
- GenerateMaze: Implements the core of Prim's Algorithm:
-- Starts from a random position and marks it as a path.
-- Adds neighboring walls to a list.
-- Randomly selects a wall, checks if it can be converted into a path (by ensuring it connects to an already carved path), and updates the maze.
-- Repeats until no more walls are left to process.
- Maze Generation with Mesh Combination:
-- A 3D cube is created for each wall in the grid.
-- The walls are combined into a single mesh for optimization, improving performance by reducing the number of game objects in the scene.

Breakdown of Methods:
- CalculateNeighbors(Vector2Int position, bool returnPaths):
-- Takes the current cell position.
-- Checks each neighboring cell (north, south, east, west) with a distance of 2 units to ensure there’s space for walls between cells.
-- If returnPaths is true, it returns neighbors that are part of the path. If false, it returns neighbors that are walls.
- GenerateMaze():
-- Initializes the maze by marking the starting cell as part of the path.
-- Iteratively processes walls and carves new paths until the maze is fully generated.
After maze generation, combines all wall meshes into a single object for better performance.
- Start():
-- Initializes the grid and triggers the maze generation when the script starts.

Optimization:
Mesh Combination: Instead of having hundreds or thousands of individual wall GameObjects, this script combines all the wall meshes into a single mesh, 
which reduces draw calls and improves performance.
Randomness: The use of Random.Range introduces randomness into the maze generation, ensuring that each run results in a unique maze structure.

Prim's Algorithm Summary (Adapted for Maze Generation)
- Start with a grid where all cells are walls.
- Pick a random cell, mark it as part of the maze, and add its neighboring walls to a list.
- Randomly pick a wall from the list and check if it divides a path from an unvisited cell.
- If yes, carve through the wall and make the unvisited cell part of the maze.
- Add the neighboring walls of the new cell to the list.
- Repeat until no more walls are left in the list.
*/