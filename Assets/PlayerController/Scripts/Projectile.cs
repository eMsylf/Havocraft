using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class Projectile : MonoBehaviour
{
    public Player Owner;
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

    public float Damage = 1f;

    protected Vector3 impactPosition;
    private float startTime = 0f;
    private Vector3 startPos;
    private bool armed = false;
    private bool hasImpacted = false;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    Rigidbody rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public Rigidbody Rigidbody
    {
        get
        {
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody>();
            return rigidbody;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (armed)
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
        //Debug.Log("Arm " + name);
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
                if (!CanHitOwner)
                {
                    if (collisionData.gameObject == Owner.gameObject)
                    {
                        Debug.Log(name + " can't hit owner " + Owner.name);
                        return;
                    }
                }
                output += Owner.name + " with ";
            }
            output += name;
            Debug.Log(collisionData.gameObject.name + " was hit by " + output);
            Player playerHit = collisionData.gameObject.GetComponent<Player>();
            if (playerHit != null)
            {
                //player.TakeDamage(Damage);
                if (Owner != null)
                {
                    GameManager.Instance.PlayerTakesDamage(playerHit, Damage, Owner);

                }
                GameManager.Instance.PlayerTakesDamage(playerHit, Damage);
                //player.Die();
            }
        }

        impactPosition = transform.position;
        hasImpacted = true;
        Explosive explosive = GetComponent<Explosive>();
        if (explosive != null)
        {
            explosive.Explode();
        }

        if (DisappearUponImpact)
        {
            if (ServerBehaviour.HasActiveInstance())
            {
                ServerBehaviour.Instance.SendProjectileImpact(this);
            }
            //Debug.Log("Disappear");
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
