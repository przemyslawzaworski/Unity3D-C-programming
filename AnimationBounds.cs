// Create bounding box for all frames of animation, example animation state has name "Open".
using UnityEngine;

public class AnimationBounds : MonoBehaviour
{
	[SerializeField] Animator _Animator;
	[SerializeField] MeshRenderer _MeshRenderer;

	int _Frame = 0;
	float _Length = 0f, _FrameCount = 0f, _FrameRate = 0f;
	Bounds _Bounds = new Bounds (Vector3.zero, Vector3.zero);

	void Start()
	{
		_Length = _Animator.runtimeAnimatorController.animationClips[0].length;
		_FrameRate = _Animator.runtimeAnimatorController.animationClips[0].frameRate;
		_FrameCount = _Length * _FrameRate;
	}

	void CalculateBounds()
	{
		_Frame = 0;
		_Animator.Rebind();
		_Animator.Update(0f);
		_Bounds = _MeshRenderer.bounds;
		for (int i = 0; i < _FrameCount; i++)
		{
			_Animator.Play("Open", -1, (float) _Frame / (float) _FrameCount);
			_Animator.Update(1.0f / _FrameRate);
			_Bounds.Encapsulate(_MeshRenderer.bounds);
			_Frame++;
		}
		_Animator.Rebind();
		_Animator.Update(0f);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			CalculateBounds();
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(_Bounds.center, _Bounds.size);
	}

	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 200, 20), "Bounds size: " + _Bounds.size.ToString());
	}
}