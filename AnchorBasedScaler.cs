using UnityEngine;

public class AnchorBasedScaler : MonoBehaviour
{
	[SerializeField] private Transform _Box;
	[SerializeField] private Axis _Axis;
	private float _Scale = 1.0f;
	public enum Axis {X, Y, Z};

	/// <summary>
	/// Scales the given box Transform along a specified axis while keeping one side fixed.
	/// </summary>	
	public void ScaleBox(Transform box, float newScaleValue, Axis axis, bool anchorMin = true)
	{
		Vector3 oldScale = box.localScale;
		Vector3 newScale = oldScale;
		Vector3 newPosition = box.position;
		switch (axis)
		{
			case Axis.X:
				float deltaX = newScaleValue - oldScale.x;
				newScale.x = newScaleValue;
				newPosition.x += anchorMin ? deltaX / 2f : -deltaX / 2f;
				break;
			case Axis.Y:
				float deltaY = newScaleValue - oldScale.y;
				newScale.y = newScaleValue;
				newPosition.y += anchorMin ? deltaY / 2f : -deltaY / 2f;
				break;
			case Axis.Z:
				float deltaZ = newScaleValue - oldScale.z;
				newScale.z = newScaleValue;
				newPosition.z += anchorMin ? deltaZ / 2f : -deltaZ / 2f;
				break;
			default:
				Debug.LogError("Invalid axis specified. Use \"x\", \"y\", or \"z\".");
				return;
		}
		box.localScale = newScale;
		box.position = newPosition;
	}

	void Start()
	{
		_Scale = _Axis == Axis.X ? _Box.localScale.x : _Axis == Axis.Y ? _Box.localScale.y : _Box.localScale.z;
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.O))
		{
			_Scale = _Scale + 0.001f;
			_Scale = Mathf.Max(_Scale, 0.0f);
			ScaleBox(_Box, _Scale, _Axis);
		}		
		if (Input.GetKey(KeyCode.P))
		{
			_Scale = _Scale - 0.001f;
			_Scale = Mathf.Max(_Scale, 0.0f);
			ScaleBox(_Box, _Scale, _Axis);
		}
	}
}