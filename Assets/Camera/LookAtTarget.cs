using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CanEditMultipleObjects]
#endif
public class LookAtTarget : MonoBehaviour
{
    [Tooltip("Uses the main camera by default")]
    public Transform OverrideTarget;

    private Transform target;
    private Transform GetTarget()
    {
        target = OverrideTarget == null ? Camera.main.transform : OverrideTarget;
        return target;
    }

    public enum Method
    {
        AlignRotation = default,
        Forward
    }
    public Method method = default;
    [Header("Only works with 'Forward'")]
    public bool reverse;
    public bool StayUpright;

    private Vector3 lookVector;

    private void Update()
    {
        switch (method)
        {
            case Method.AlignRotation:
                transform.rotation = GetTarget().transform.rotation;
                return;
        }

        Vector3 targetPosition = GetTarget().transform.position;

        if (StayUpright) targetPosition.y = transform.position.y;

        lookVector = transform.position - targetPosition;
        if (reverse) lookVector *= -1f;
        switch (method)
        {
            case Method.Forward:
                transform.forward = lookVector;
                break;
        };
    }
}
