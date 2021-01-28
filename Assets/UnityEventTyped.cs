using System;

namespace UnityEngine.Events
{
    [Serializable]
    public class UnityEventGameObject : UnityEvent<GameObject> { }

    [Serializable]
    public class UnityEventFloat : UnityEvent<float> { }

    [Serializable]
    public class UnityEventInt : UnityEvent<int> { }

    [Serializable]
    public class UnityEventString : UnityEvent<string> { }

    [Serializable]
    public class UnityEventBool : UnityEvent<bool> { }

    [Serializable]
    public class UnityEventVector2 : UnityEvent<Vector2> { }

    [Serializable]
    public class UnityEventCustomType<T> : UnityEvent<T> { }

    [Serializable]
    public class UnityEventData
    {
        public string EventString;
        public float EventFloat;
        public int EventInt;
        public GameObject EventObject;
        public Color color;
    }

    [Serializable]
    public class UnityEventWithData {
        public UnityEventData eventData = new UnityEventData();
        public DataEvent Event;
    }

    [Serializable]
    public class DataEvent : UnityEvent<UnityEventData>
    {

    }
}
