// UI -> Button
// Execute a function after button click, then show button image name
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class button : MonoBehaviour 
{
	public Button DebugButton;
	
	void Start () 
	{
		DebugButton.onClick.AddListener(ShowCaption);
	}
	
	void ShowCaption () 
	{
		string ImageName = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite.name;
		Debug.Log(ImageName);
	}
}