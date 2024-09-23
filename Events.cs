using System;
using UnityEngine;
using UnityEngine.Events;

public class Events : MonoBehaviour
{
    private event Action<float> OnActionExecute = delegate { }; // Action-based event
    private UnityEvent<float> OnUnityExecute; // UnityEvent-based event
    private event EventHandler<CustomEventArgs> OnEventHandlerExecute; // EventHandler-based event

    public class CustomEventArgs : EventArgs
    {
        public float Value { get; private set; }
        public CustomEventArgs(float value) => Value = value;
    }

    void Start()
    {
        OnActionExecute += HandleActionEvent;
        OnUnityExecute = new UnityEvent<float>();
        OnUnityExecute.AddListener(HandleUnityEvent);
        OnEventHandlerExecute += HandleEventHandler;
        Debug.Log("Press 'I' for Action event, 'O' for UnityEvent, 'P' for EventHandler");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            TriggerActionEvent(1.0f);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            TriggerUnityEvent(2.0f);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            TriggerEventHandler(3.0f);
        }
    }

    void TriggerActionEvent(float value)
    {
        OnActionExecute.Invoke(value);
    }

    void TriggerUnityEvent(float value)
    {
        OnUnityExecute.Invoke(value);
    }

    void TriggerEventHandler(float value)
    {
         OnEventHandlerExecute.Invoke(this, new CustomEventArgs(value));
    }

    void HandleActionEvent(float value)
    {
        Debug.Log($"Action Event triggered with value: {value}");
    }

    void HandleUnityEvent(float value)
    {
        Debug.Log($"UnityEvent triggered with value: {value}");
    }

    void HandleEventHandler(object sender, CustomEventArgs e)
    {
        Debug.Log($"EventHandler triggered by {sender} with value: {e.Value}");
    }

    void OnDestroy()
    {
        OnActionExecute -= HandleActionEvent;
        OnUnityExecute.RemoveListener(HandleUnityEvent);
        OnEventHandlerExecute -= HandleEventHandler;
    }
}