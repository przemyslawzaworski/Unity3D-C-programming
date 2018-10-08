using UnityEngine;
using UnityEngine.UI;

public class slider : MonoBehaviour 
{
	public GameObject Target;
	public Slider MainSlider;
	
	void Start () 
	{
		MainSlider.onValueChanged.AddListener(delegate {Shift();} );		
	}
	
	void Shift ()
	{
		Target.transform.position = new Vector3(MainSlider.value, 0, 0);
	}
}
