using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTargetGroup : MonoBehaviour
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
    public enum UpOrientation
    {
        Up,
        Right,
        Forward
    }
    public UpOrientation upOrientation;
    public bool ReverseUp;

    private void Update()
    {
        DoLookAt();
    }

    public void DoLookAt()
    {
        switch (method)
        {
            case Method.AlignRotation:
                foreach (Transform trans in transform)
                {
                    trans.forward = GetTarget().forward;
                }
                break;

            case Method.Forward:
                Vector3 targetPosition = GetTarget().position;
                Vector3 up;
                switch (upOrientation)
                {
                    case UpOrientation.Up:
                        up = Vector3.up;
                        break;
                    case UpOrientation.Right:
                        up = Vector3.right;
                        break;
                    case UpOrientation.Forward:
                        up = Vector3.forward;
                        break;
                    default:
                        up = Vector3.up;
                        break;
                }
                if (ReverseUp) up *= -1f;
                foreach (Transform trans in transform)
                {
                    Vector3 lookPos = targetPosition;
                    if (StayUpright) lookPos.y = 0f;
                    
                    if (reverse) lookPos = trans.position + target.position;

                    trans.LookAt(lookPos, up);
                }
                break;
        }
    }
}
