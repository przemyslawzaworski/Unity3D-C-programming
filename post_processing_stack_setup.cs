//Player Settings should has Scripting Define Symbol: UNITY_POST_PROCESSING_STACK_V1

using UnityEngine;
using UnityEngine.PostProcessing;

public class post_processing_stack_setup : MonoBehaviour 
{
	public PostProcessingProfile PPP;
	
	void Start ()
	{
		PPP.colorGrading.enabled = true;		
	}
	
	void Update () 
	{
		ColorGradingModel.Settings cgms = PPP.colorGrading.settings;
		cgms.basic.saturation = Mathf.Lerp(0.0f,1.0f,Mathf.Sin(Time.time)*0.5f+0.5f);
		PPP.colorGrading.settings = cgms;		
	}
}
