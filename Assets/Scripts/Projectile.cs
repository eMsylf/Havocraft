using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public PlayerController Owner;
    public float ExplosionForce = 10f;
    public float ExplosionRadius = 2f;
    public float UpwardsModifier = 1f;

    Vector3 start;
    Vector3 impact;
    bool hasImpacted = false;

    public List<Rigidbody> ExplodedRbList = new List<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        string output = "";
        if (Owner != null)
        {
            output += Owner.name + " with ";
        }
        output += name;
        Debug.Log(collision.gameObject.name + " was hit by " + output);

        impact = transform.position;
        hasImpacted = true;
        foreach (Collider col in Physics.OverlapSphere(impact, ExplosionRadius))
        {
            if (col.attachedRigidbody != null)
            {
                col.attachedRigidbody.AddExplosionForce(ExplosionForce, impact, ExplosionRadius, UpwardsModifier);
                col.attachedRigidbody.AddTorque(ExplosionForce, 0f, 0f);
                ExplodedRbList.Add(col.attachedRigidbody);
                Debug.Log("Add explosion force to " + col.name, col);
            }
        }
        //collision.rigidbody?.AddExplosionForce(ExplosionForce, impact, ExplosionRadius, UpwardsModifier);

        // Disable the projectile
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void Start()
    {
        start = transform.position;
    }


    private void OnDrawGizmos()
    {
        if (!hasImpacted)
            Gizmos.DrawRay(transform.position, GetComponent<Rigidbody>().velocity);
        Gizmos.DrawLine(start, transform.position);

        foreach (Rigidbody rb in ExplodedRbList)
        {
            Gizmos.DrawLine(transform.position, rb.position);
        }
    }
}
