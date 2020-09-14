using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class Projectile : MonoBehaviour
{
    public PlayerController Owner;
    public bool CanHitOwner = false;
    [Tooltip("The arm delay allows a distance or time to be set, to prevent the projectile to collide with the ")]
    public bool ArmDelay = true;
    public ArmType ArmType;
    [Min(0f)]
    public float ArmDelayTime = .1f;
    [Min(0f)]
    public float ArmDelayDistance = .1f;
    [Min(0f)]
    public float MaxLifetime = 5f;

    public bool DisappearUponImpact = true;
    
    protected Vector3 impact;
    private float startTime = 0f;
    private Vector3 startPos;
    private bool armed = false;
    private bool hasImpacted = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.attachedRigidbody.GetComponent<PlayerController>() != Owner)
            Impact(collision);
    }

    private void Start()
    {
        startPos = transform.position;
        startTime = Time.time;
        if (ArmDelay) GetComponent<Collider>().enabled = false;
    }

    private void Update()
    {
        if (!armed)
        {
            switch (ArmType)
            {
                case ArmType.Distance:
                    if (Vector3.Distance(startPos, transform.position) > ArmDelayDistance)
                    {
                        Arm(true);
                    }
                    break;
                case ArmType.Time:
                    if (Time.time - startTime > ArmDelayTime)
                    {
                        Arm(true);
                    }
                    break;
                default:
                    break;
            }
        }

        if (Time.time - startTime > MaxLifetime)
        {
            Impact(null);
        }
    }

    private void OnDrawGizmos()
    {
        if (!hasImpacted)
            Gizmos.DrawRay(transform.position, GetComponent<Rigidbody>().velocity);
        Gizmos.DrawLine(startPos, transform.position);
    }

    private void Arm(bool state)
    {
        Debug.Log("Arm " + name);
        armed = state;
        GetComponent<Collider>().enabled = state;
    }

    protected virtual void Impact(Collision collisionData)
    {
        if (collisionData != null)
        {
            string output = "";
            if (Owner != null)
            {
                output += Owner.name + " with ";
            }
            output += name;
            Debug.Log(collisionData.gameObject.name + " was hit by " + output);
        }

        impact = transform.position;
        hasImpacted = true;
        Explosive explosive = GetComponent<Explosive>();
        if (explosive != null)
        {
            explosive.Explode();
        }
        if (DisappearUponImpact)
        {
            gameObject.SetActive(false);
        }
    }
}

public enum ArmType
{
    Distance,
    Time
}

[System.Serializable]
public class ExplosionData
{
    public bool Explosive = false;
    public float Force = 10f;
    public float Radius = 2f;
}
