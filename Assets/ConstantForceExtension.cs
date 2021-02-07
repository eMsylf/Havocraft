using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(ConstantForce))]
public class ConstantForceExtension : MonoBehaviour
{

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    ConstantForce constantForce;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    ConstantForce GetConstantForce()
    {
        if (constantForce == null) constantForce = GetComponent<ConstantForce>();
        return constantForce;
    }
    public Vector3 referenceForce = new Vector3();

    private void FixedUpdate()
    {
        ConstantForce cf = GetConstantForce();
        if (ApplyToForce)
            cf.force = CalculateAlteredForce(referenceForce);
        if (ApplyToRelativeForce)
            cf.relativeForce = CalculateAlteredForce(referenceForce);
        if (ApplyToTorque)
            cf.torque = CalculateAlteredForce(referenceForce);
        if (ApplyToRelativeTorque)
            cf.relativeTorque = CalculateAlteredForce(referenceForce);
        //GetConstantForce().relativeForce.y;
    }

    public bool ApplyToForce;
    public bool ApplyToRelativeForce;
    public bool ApplyToTorque;
    public bool ApplyToRelativeTorque;

    public enum SurfaceCheckType
    {
        Down,
        //Sphere
    }
    public SurfaceCheckType surfaceCheckType;
    [Min(.0000001f)]
    public float CheckRange = 1f;
    private float CheckSurfaceProximity()
    {
        float distance;
        switch (surfaceCheckType)
        {
            default:
            case SurfaceCheckType.Down:
                Ray ray = new Ray(transform.position, -transform.up);
                bool isHit = Physics.Raycast(ray, out RaycastHit hitInfo, CheckRange);
                distance = isHit ? hitInfo.distance:CheckRange;
                break;
            //case SurfaceCheckType.Sphere:
            //    isHit = Physics.CheckSphere(transform.position, CheckRange);
            //    distance = isHit?0f:CheckRange;
            //    break;
        }
        distance = Mathf.Clamp(distance, 0f, CheckRange);
        return distance;
    }

    public AnimationCurve proximityForceCurve = new AnimationCurve(default);

    private Vector3 CalculateAlteredForce(Vector3 reference)
    {
        float distance = CheckSurfaceProximity();
        //Debug.Log("Distance" + distance);
        Vector3 force = proximityForceCurve.Evaluate(1-distance/CheckRange) * reference * CheckRange;
        return force;
    }

    private Vector3 Endpoint()
    {
        return transform.position - transform.up * CheckRange;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        switch (surfaceCheckType)
        {
            default:
            case SurfaceCheckType.Down:
                Gizmos.DrawLine(transform.position, Endpoint());
                Handles.DrawSolidDisc(Endpoint(), transform.up, .5f);
                break;
            //case SurfaceCheckType.Sphere:
            //    Gizmos.DrawWireSphere(transform.position, CheckRange);
            //    break;
        }
    }
#endif
}
