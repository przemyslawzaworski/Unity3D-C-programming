// Check whether entire mesh is inside frustum.
using UnityEngine;

public class Frustum : MonoBehaviour
{
	public GameObject Source;
	private Mesh _Mesh;

	Vector3 WorldToViewportPoint(Vector3 vector3, Camera camera)
	{
		Vector4 worldPos = new Vector4(vector3.x, vector3.y, vector3.z, 1.0f);
		Vector4 viewPos = camera.worldToCameraMatrix * worldPos;
		Vector4 projPos = camera.projectionMatrix * viewPos;
		Vector3 ndcPos = new Vector3(projPos.x / projPos.w, projPos.y / projPos.w, projPos.z / projPos.w);
		return new Vector3((ndcPos.x + 1.0f) * 0.5f, (ndcPos.y + 1.0f) * 0.5f, -viewPos.z);
	}

	bool IsPointInsideFrustum (Vector3 vector3, Camera camera)
	{
		Vector3 p = WorldToViewportPoint(vector3, camera);
		return (p.x > 0.0f && p.x < 1.0f && p.y > 0.0f && p.y < 1.0f && p.z > 0.0f);
	}

	bool IsMeshInsideFrustum (Mesh mesh, Transform transform, Camera camera)
	{
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 worldPos = transform.TransformPoint(vertices[i]);
			bool result = IsPointInsideFrustum (worldPos, Camera.main);
			if (result == false) return false;
		}
		return true;
	}

	void Start()
	{
		_Mesh = Source.GetComponent<MeshFilter>().sharedMesh;
	}

	void Update()
	{
		bool result = IsMeshInsideFrustum (_Mesh, Source.transform, Camera.main);
		Debug.Log(result);
	}
}