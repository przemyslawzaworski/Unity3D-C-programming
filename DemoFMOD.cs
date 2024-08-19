using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DemoFMOD : MonoBehaviour
{
    FMOD.Studio.EventInstance _EventInstance;
    FMOD.Studio.PARAMETER_ID _ID;
    float _EventInstanceUse = 0.0f;    
    
    FMOD.Studio.PARAMETER_ID CacheHandle(FMOD.Studio.EventInstance eventInstance, string parameterName)
    {
        FMOD.Studio.EventDescription eventDescription;
        eventInstance.getDescription(out eventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION parameterDescription;
        eventDescription.getParameterDescriptionByName(parameterName, out parameterDescription);
        return parameterDescription.id;
    }

    bool IsPlaying(FMOD.Studio.EventInstance eventInstance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        eventInstance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }
    
    void Start()
    {
        _EventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Audio/EventName");
        _EventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        _ID = CacheHandle(_EventInstance, "ParamaterName");
    }
    
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 150, 200, 50), "Enable Audio"))
        {
            if (!IsPlaying(_EventInstance))
            {
                _EventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
                _EventInstance.start();
                _EventInstanceUse = 1.0f;
                _EventInstance.setParameterByID(_ID, _EventInstanceUse);
            }
        }    
    }
    
    void OnDestroy()
    {
        _EventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _EventInstance.release();
    }
}