using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Explosive : MonoBehaviour
{
    public float Force = 10f;
    public float UpwardsModifier = 1f;
    public float Radius = 2f;

    public List<Rigidbody> ExplodedRbList = new List<Rigidbody>();

    public void Explode()
    {
        foreach (Collider col in Physics.OverlapSphere(transform.position, Radius))
        {
            if (col.attachedRigidbody != null)
            {
                col.attachedRigidbody.AddExplosionForce(Force, transform.position, Force, UpwardsModifier);
                col.attachedRigidbody.AddTorque(Force, 0f, 0f);
                ExplodedRbList.Add(col.attachedRigidbody);
                Debug.Log("Add explosion force to " + col.name, col);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Rigidbody rb in ExplodedRbList)
        {
            Gizmos.DrawLine(transform.position, rb.position);
        }
    }
}
