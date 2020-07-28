using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public PlayerController Owner;

    Vector3 start;
    Vector3 impact;
    bool hasImpacted = false;

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

        // Disable the object
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
    }
}
