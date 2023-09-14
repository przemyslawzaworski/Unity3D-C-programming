using UnityEngine;

// Calculate the centroid of the surface area of the mesh.
public class MeshCentroid : MonoBehaviour
{
	public GameObject Source;

	Vector3 Calculate(MeshFilter meshFilter, Transform transform)
	{
		float totalArea = 0.0f;
		Vector3 centroid = new Vector3(0.0f, 0.0f, 0.0f);
		Mesh mesh = meshFilter.sharedMesh;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			Vector3 a = transform.TransformPoint(vertices[triangles[i + 0]]);
			Vector3 b = transform.TransformPoint(vertices[triangles[i + 1]]);
			Vector3 c = transform.TransformPoint(vertices[triangles[i + 2]]);
			Vector3 center = (a + b + c) / 3f;
			float area = 0.5f * Vector3.Cross(b - a, c - a).magnitude;
			centroid += area * center;
			totalArea += area;
		}
		centroid /= totalArea;
		return centroid;
	}

	void Start()
	{
		MeshFilter meshFilter = Source.GetComponent<MeshFilter>();
		if (meshFilter)
			Debug.Log("Centroid of mesh: " + Calculate(meshFilter, Source.transform).ToString());
	}
}