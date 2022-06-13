using UnityEngine;

// Calculate volume of the polyhedron.
public class MeshVolume : MonoBehaviour
{
	public GameObject Source;

	float SignedVolumeOfTetrahedron(Vector3 v1, Vector3 v2, Vector3 v3) 
	{
		float a = v3.x * v2.y * v1.z;
		float b = v2.x * v3.y * v1.z;
		float c = v3.x * v1.y * v2.z;
		float d = v1.x * v3.y * v2.z;
		float e = v2.x * v1.y * v3.z;
		float f = v1.x * v2.y * v3.z;
		return (1.0f / 6.0f) * (-a + b + c - d - e + f);
	}

	float Calculate(MeshFilter meshFilter)
	{
		float volume = 0.0f;
		Mesh mesh = meshFilter.sharedMesh;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		for (int i = 0; i < mesh.triangles.Length; i += 3)
		{
			Vector3 p1 = meshFilter.gameObject.transform.TransformPoint(vertices[triangles[i + 0]]);
			Vector3 p2 = meshFilter.gameObject.transform.TransformPoint(vertices[triangles[i + 1]]);
			Vector3 p3 = meshFilter.gameObject.transform.TransformPoint(vertices[triangles[i + 2]]);
			volume += SignedVolumeOfTetrahedron(p1, p2, p3);
		}
		return Mathf.Abs(volume);
	}

	void Start()
	{
		MeshFilter meshFilter = Source.GetComponent<MeshFilter>();
		if (meshFilter)
			Debug.Log("Volume of mesh: " + Calculate(meshFilter).ToString());
	}
}