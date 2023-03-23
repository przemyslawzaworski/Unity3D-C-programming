using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Touchpad : MonoBehaviour
{
	public RawImage MovementImage;
	public RawImage RotationImage;
	public GameObject Player;
	public float MovementSpeed = 5.0f;
	public float RotationSpeed = 100.0f;

	Vector3[] _TranslationCorners = new Vector3[4];
	Vector3[] _RotationCorners = new Vector3[4];
	float rotationY = 0F;

	void SetTranslation()
	{
		MovementImage.rectTransform.GetWorldCorners(_TranslationCorners);
		Rect rect = Rect.zero;
		rect.position = _TranslationCorners[0];
		rect.size = _TranslationCorners[2] - _TranslationCorners[0];
		Vector2 mouse = Vector2.zero;
		mouse.x = Input.mousePosition.x - rect.x;
		mouse.y = Input.mousePosition.y - rect.y;
		Vector2 resolution = Vector2.zero;
		resolution.x = MovementImage.rectTransform.rect.width;
		resolution.y = MovementImage.rectTransform.rect.height;
		if (mouse.x >= 0f &&  mouse.x <= resolution.x && mouse.y >= 0f && mouse.y <= resolution.y)
		{
			Vector2 direction = new Vector2 (mouse.x - (resolution.x / 2f), mouse.y - (resolution.y / 2f)).normalized;
			Vector3 offset = (direction.x * Player.transform.right + direction.y * Player.transform.forward) * Time.deltaTime * MovementSpeed;
			Player.transform.position += new Vector3(offset.x, 0f, offset.z);
		}
	}

	void SetRotation()
	{
		RotationImage.rectTransform.GetWorldCorners(_RotationCorners);
		Rect rect = Rect.zero;
		rect.position = _RotationCorners[0];
		rect.size = _RotationCorners[2] - _RotationCorners[0];
		Vector2 mouse = Vector2.zero;
		mouse.x = Input.mousePosition.x - rect.x;
		mouse.y = Input.mousePosition.y - rect.y;
		Vector2 resolution = Vector2.zero;
		resolution.x = RotationImage.rectTransform.rect.width;
		resolution.y = RotationImage.rectTransform.rect.height;
		if (mouse.x >= 0f &&  mouse.x <= resolution.x && mouse.y >= 0f && mouse.y <= resolution.y)
		{
			Vector2 direction = new Vector2 (mouse.x - (resolution.x / 2f), mouse.y - (resolution.y / 2f)).normalized;
			float sensitivity = RotationSpeed * Time.deltaTime;
			float rotationX = Player.transform.localEulerAngles.y + direction.x * sensitivity;
			rotationY += direction.y * sensitivity;
			rotationY = Mathf.Clamp (rotationY, -60F, 60F);
			Player.transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
	}	

	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			SetTranslation();
			SetRotation();
		}
	}
}