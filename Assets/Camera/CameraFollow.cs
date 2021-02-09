using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Rigidbody))]
public class CameraFollow : MonoBehaviour
{
    //[SerializeField] private Transform target;
    //public Transform Target
    //{
    //    get
    //    {
    //        if (target == null)
    //        {
    //            Debug.LogWarning("No target assigned in " + name, this);
    //        }
    //        return target;
    //    }
    //    set => target = value;
    //}

    public List<Transform> Targets = new List<Transform>();

    [SerializeField] private Vector3 targetPosition;
    public Vector3 TargetPosition
    {
        get
        {
            switch (method)
            {
                case Method.AsChild:
                    targetPosition = simulatedParent.position;
                    break;
                case Method.CameraResources:
                    if (Targets != null && Targets.Count != 0)
                    {
                        targetPosition = GetCenterPosition(Targets);
                    }
                    break;
                default:
                    break;
            }
            return targetPosition;
        }
        set
        {
            targetPosition = value;
        }
    }
    // Code for getting the center position of multiple points from: https://stackoverflow.com/questions/52375649/get-the-center-point-between-many-gameobjects-in-unity
    public Vector3 GetCenterPosition(List<Transform> transforms)
    {
        Vector3 center = new Vector3();
        if (transforms == null || transforms.Count == 0)
        {
            return center;
        }
        for (int i = 0; i < transforms.Count; i++)
        {
            center += transforms[i].position;
        }

        center /= transforms.Count;
        return center ;
    }
    public enum Method
    {
        AsChild,
        CameraResources
    }
    public Method method = Method.CameraResources;

    [Header("As Child")]
    public Transform simulatedParent;
    [Header("Camera Resources")]
    [SerializeField] private Transform cameraResources;
    public Transform CameraResources
    {
        get
        {
            if (cameraResources == null)
            {
                cameraResources = new GameObject("Camera resources").transform;
            }
            return cameraResources;
        }
    }

    [Space]

    [SerializeField] private Transform positionPivot;
    public Transform PositionPivot
    {
        get
        {
            if (positionPivot == null)
            {
                positionPivot = new GameObject("Position pivot").transform;
                positionPivot.parent = CameraResources;
            }
            return positionPivot;
        }
    }

    [SerializeField] private Transform positionTransform;
    public Transform PositionTransform
    {
        get
        {
            if (positionTransform == null)
            {
                positionTransform = new GameObject("Position transform").transform;
                positionTransform.parent = PositionPivot;
            }
            return positionTransform;
        }
    }

    [SerializeField] private Transform lookTransform;
    public Transform LookTransform
    {
        get
        {
            if (lookTransform == null)
            {
                lookTransform = new GameObject("Look transform").transform;
                lookTransform.parent = CameraResources;
            }
            return lookTransform;
        }
    }
    [Header("Position")]
    public Vector3 PositionOffset;
    [Range(0f, .99f)]
    public float PositionSmoothing = .125f;
    public bool PlaceBeforeObstacles = true;
    public LayerMask ObstacleMask;

    [Header("Rotation")]
    public Vector3 RotationAroundTarget;
    [Header("Distance")]
    [Min(0f)]
    public float Distance = 2f;
    [Header("Look")]
    public Vector3 LookOffset;
    [Range(0f, .99f)]
    public float LookSmoothing = .125f;

    [Header("Live preview")]
    public bool EditorPreview = false;

    private new Rigidbody rigidbody;
    private Rigidbody Rigidbody
    {
        get
        {
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody>();
            return rigidbody;
        }
    }

    private void Start()
    {
        EditorPreview = false;
    }

    private void FixedUpdate()
    {
        SetPositionAndLook();
    }

    public void SetPositionAndLook()
    {
        switch (method)
        {
            case Method.AsChild:
                SetPosition(simulatedParent.position);
                SetLookTo(simulatedParent);
                break;
            case Method.CameraResources:
                CalculatePivot();
                SetPosition(CalculatePosition());
                CalculateRotationAroundTarget();
                SetLook(CalculateLook());
                break;
            default:
                break;
        }
    }

    public void CalculatePivot()
    {
        PositionPivot.position = TargetPosition + PositionOffset;
    }

    public Vector3 CalculatePosition()
    {
        if (PlaceBeforeObstacles)
        {
            float overrideDistance = Distance;
            Ray cameraDistanceRay = new Ray(PositionPivot.position, -PositionPivot.forward);
            if (Physics.Raycast(cameraDistanceRay, out RaycastHit info, Distance, ObstacleMask))
            {
                overrideDistance = info.distance;
            }
            PositionTransform.position = PositionPivot.position - PositionPivot.forward * overrideDistance;
        }
        else 
            PositionTransform.position = PositionPivot.position - PositionPivot.forward * Distance;
        return PositionTransform.position;
    }

    private void SetPosition(Vector3 position)
    {
        if (EditorPreview)
            transform.position = Vector3.Lerp(Rigidbody.position, position, 1f - PositionSmoothing);
        else
            Rigidbody.MovePosition(Vector3.Lerp(Rigidbody.position, position, 1f - PositionSmoothing));
    }

    private void CalculateRotationAroundTarget()
    {
        PositionPivot.rotation = Quaternion.Euler(RotationAroundTarget);
    }

    private Transform CalculateLook()
    {
        // Get camera look position
        Vector3 desiredLook = TargetPosition + LookOffset;
        if (LookSmoothing == 0f)
            LookTransform.position = desiredLook;
        else
            LookTransform.position = Vector3.Lerp(lookTransform.position, desiredLook, 1f - LookSmoothing);
        return LookTransform;
    }

    private void SetLookTo(Transform rottationTransform)
    {
        Rigidbody.MoveRotation(Quaternion.Lerp(Rigidbody.rotation, rottationTransform.rotation, 1f - LookSmoothing));
    }

    private void SetLook(Transform lookTarget)
    {
        // Camera looks at the target position
        if (EditorPreview)
            transform.LookAt(lookTarget.position);
        else
            Rigidbody.MoveRotation(Quaternion.LookRotation(lookTarget.position - Rigidbody.position));
    }

    //public void OverrideOffsets()
    //{
    //    //PositionTransform.position = transform.position;
    //    PositionOffset = Target.position - transform.position;
    //}

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 targetPos = TargetPosition;
        Gizmos.DrawWireSphere(targetPos, 1f);
        switch (method)
        {
            case Method.AsChild:
                break;
            case Method.CameraResources:
                Gizmos.DrawLine(transform.position, targetPos);
                Gizmos.DrawLine(PositionTransform.position, targetPos);
                Gizmos.DrawLine(LookTransform.position, targetPos);
                Gizmos.DrawLine(PositionTransform.position, PositionPivot.position);
                Gizmos.DrawLine(PositionPivot.position, TargetPosition);
                break;
            default:
                break;
        }
    }
#endif
}
