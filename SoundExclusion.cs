/*
When player is inside volume defined by 3D independent box colliders, script turns off audio,
otherwise turns on audio. Author: Przemyslaw Zaworski
*/

using UnityEngine;

public class SoundExclusion : MonoBehaviour
{
	public Camera MainCamera;
	public AudioSource[] AudioObjects;
	public BoxCollider[] Colliders;
	bool _InsideExclusionZone = false;
	bool _State = false;

	// Returns positive number when point p is outside cube,
	// returns negative number when point p is inside cube.
	float Box (Vector3 p, Vector3 c, Vector3 s)
	{
		float x = Mathf.Max(p[0] - c[0] - s[0], c[0] - p[0] - s[0]);
		float y = Mathf.Max(p[1] - c[1] - s[1], c[1] - p[1] - s[1]);   
		float z = Mathf.Max(p[2] - c[2] - s[2], c[2] - p[2] - s[2]);
		return Mathf.Max(Mathf.Max(x,y),z);
	}

	void Update()
	{
		for (int i=0; i<Colliders.Length; i++)
		{
			Vector3 c = Colliders[i].transform.TransformPoint(Colliders[i].center);
			Vector3 s = Vector3.Scale(Colliders[i].size, Colliders[i].transform.localScale) * 0.5f;
			float d = Box (MainCamera.transform.position, c, s);
			_InsideExclusionZone = (d < 0.0f);
			if (_InsideExclusionZone) break;
		}
		if (_State != !_InsideExclusionZone)
		{
			for (int i=0; i<AudioObjects.Length; i++)
			{
				_State = AudioObjects[i].enabled = !_InsideExclusionZone;
			}
		}
	}
}
