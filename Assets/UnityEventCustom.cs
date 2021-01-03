using UnityEngine;
using UnityEngine.Events;
using System.Collections;
[System.Serializable]
public class UnityEventCustom : UnityEvent<EventData>
{
    public EventData eventData;
}

[System.Serializable]
public class EventData
{
    public Color myColor;
    public int myInt;
    public float myFloat;
    //etc.
}

//[System.Serializable]
//public class CustomUnityEvent : UnityEvent<EventData> { }