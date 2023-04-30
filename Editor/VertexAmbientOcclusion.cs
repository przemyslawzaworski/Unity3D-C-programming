using UnityEditor;
using UnityEngine;

public class VertexAmbientOcclusion : EditorWindow
{
	public int SampleCount = 512;
	public float RayLength = 3.0f;
	public float Intensity = 1.0f;
	public bool Blur = true;
	public bool SharedMesh = true;
	public bool AddColliders = false;
	public bool AddMaterials = false;

	[MenuItem ("GameObject/Vertex Ambient Occlusion")]
	static void ShowWindow()
	{
		EditorWindow.GetWindow (typeof(VertexAmbientOcclusion));
	}

	void OnGUI()
	{
		SampleCount = EditorGUILayout.IntField("Sample Count", SampleCount);
		RayLength = EditorGUILayout.FloatField("Ray Length", RayLength);
		Intensity = EditorGUILayout.FloatField("Intensity", Intensity);
		Blur = EditorGUILayout.Toggle("Blur", Blur);
		SharedMesh = EditorGUILayout.Toggle("Shared Mesh", SharedMesh);
		AddColliders = EditorGUILayout.Toggle("Add Colliders", AddColliders);
		AddMaterials = EditorGUILayout.Toggle("Add Materials", AddMaterials);
		if (GUILayout.Button("Bake Ambient Occlusion")) BakeAmbientOcclusion();
		if (GUILayout.Button("Save Mesh As Asset")) SaveMesh();
	}

	void BakeAmbientOcclusion()
	{
		GameObject source = Selection.activeGameObject;
		if (source == null) return;
		MeshFilter[] meshFilters = source.GetComponentsInChildren<MeshFilter>();
		int vertexCount = 0;
		int progress = 0;
		for (int m = 0; m < meshFilters.Length; m++) 
		{
			vertexCount += meshFilters[m].sharedMesh.vertices.Length;
			MeshCollider meshCollider = meshFilters[m].gameObject.GetComponent<MeshCollider>();
			if (AddColliders && (meshCollider == null)) meshFilters[m].gameObject.AddComponent<MeshCollider>();
			if (AddMaterials)
			{
				meshFilters[m].gameObject.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Vertex Ambient Occlusion"));
			}
		}
		for (int k = 0; k < meshFilters.Length; k++)
		{
			Mesh mesh = SharedMesh ? meshFilters[k].sharedMesh : Instantiate(meshFilters[k].sharedMesh);
			Vector3[] vertices = mesh.vertices;
			Vector3[] normals = mesh.normals;
			if (normals.Length == 0) mesh.RecalculateNormals();
			Color[] colors = mesh.colors;
			if (colors.Length == 0) colors = new Color[vertices.Length];
			for (int x = 0; x < colors.Length; x++) colors[x].a = 1.0f;
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3 worldPos = meshFilters[k].transform.TransformPoint(vertices[i]);
				Vector3 displacement = meshFilters[k].transform.TransformPoint(vertices[i] + normals[i]);
				Vector3 worldNormal = displacement - worldPos;
				worldNormal.Normalize();
				float occlusion = 0;
				for (int j = 0; j < SampleCount; j++)
				{
					Vector3 dir = Quaternion.Euler(Random.Range(-90f, 90f), Random.Range(-90f, 90f), Random.Range(-90f, 90f)) * Vector3.up;
					Vector3 ray = Quaternion.FromToRotation(Vector3.up, worldNormal) * dir;
					Vector3 offset = Vector3.Reflect(ray, worldNormal);
					ray = ray * RayLength / ray.magnitude;
					if (Physics.Linecast(worldPos - (offset * 0.1f), worldPos + ray, out RaycastHit hit)) 
					{
						if (hit.distance > 0.0000000001f) 
						{
							occlusion += Mathf.Clamp(1.0f - (hit.distance / RayLength), 0.0f, 1.0f);
						}
					}
				}
				colors[i].a = Mathf.Clamp(1.0f - (occlusion * Intensity / SampleCount), 0.0f, 1.0f);
				EditorUtility.DisplayProgressBar("Baking Ambient Occlusion", "Please wait...", (float)progress / (float)vertexCount);
				progress++;
			}
			if (Blur) 
			{
				int[] triangles = mesh.triangles;
				for (int i = 0; i < triangles.Length; i += 3)
				{
					int t0 = triangles[i + 0];
					int t1 = triangles[i + 1];
					int t2 = triangles[i + 2];
					Color c0 = colors[t0];
					Color c1 = colors[t1];
					Color c2 = colors[t2];
					float average = (c0.a + c1.a + c2.a) / 3;
					c0.a = c0.a + (average - c0.a) / 2;
					c1.a = c1.a + (average - c1.a) / 2;
					c2.a = c2.a + (average - c2.a) / 2;
					colors[t0] = c0;
					colors[t1] = c1;
					colors[t2] = c2;
				}
			}
			mesh.colors = colors;
			if (!SharedMesh) meshFilters[k].mesh = mesh;
		}
		EditorUtility.ClearProgressBar();
	}

	void SaveMesh()
	{
		GameObject source = Selection.activeGameObject;
		if (source == null) return;
		Mesh mesh = source.GetComponent<MeshFilter>().sharedMesh;
		AssetDatabase.CreateAsset(mesh, "Assets/" + System.Guid.NewGuid().ToString("N") + ".asset");
	}
}

/*
Algorithm generates ambient occlusion for a mesh using the vertices of that mesh. 
Ambient occlusion is a shading technique that simulates the soft shadows that occur in crevices and other areas where ambient light is occluded by nearby surfaces.
The script creates an editor window with several properties to customize the occlusion generation process. 
The properties include the number of samples to take, the length of the occlusion rays, the intensity of the occlusion, 
and options for blurring the occlusion and adding colliders and materials to the mesh.
The script starts by iterating through all of the mesh filters in the selected game object, 
counting the total number of vertices and adding colliders and materials if the corresponding options are selected. 
Then, it iterates through each mesh filter again and generates ambient occlusion for each vertex.
The occlusion is generated by casting rays in random directions around each vertex and checking if the rays hit any other objects in the scene. 
If a ray hits another object, the occlusion value for that vertex is increased by the distance between the vertex and the point where the ray hit, 
normalized by the length of the ray. The final occlusion value for each vertex is the average of the occlusion values obtained from all of the ray samples.
After the occlusion values are calculated for each vertex, they are stored in the vertex colors of the mesh. 
The colors can be blurred by averaging the occlusion values of neighboring vertices. Finally, the script can optionally save the mesh as an asset.
*/