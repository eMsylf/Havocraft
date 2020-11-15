using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
    public UnityEvent CollisionEffect;

    private void OnCollisionEnter(Collision collision)
    {
        CollisionEffect.Invoke();
    }
}
