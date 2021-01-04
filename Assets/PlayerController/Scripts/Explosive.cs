using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Explosive : MonoBehaviour
{
    public float Force = 10f;
    public float Damage = 10f;
    public float UpwardsModifier = 1f;
    public float Radius = 2f;

    public List<ParticleSystem> Particles = new List<ParticleSystem>();

    public List<Rigidbody> ExplodedRbList = new List<Rigidbody>();
    public bool EmptyListUponImpact = true;

    public void Explode()
    {
        if (EmptyListUponImpact)
            ExplodedRbList.Clear();

        foreach (ParticleSystem particle in Particles)
        {
            ParticleSystem particleSystem1 = Instantiate(particle, transform.position, Quaternion.identity);
            Debug.Log("Instantiate particle", particleSystem1);
        }
        foreach (Collider col in Physics.OverlapSphere(transform.position, Radius))
        {
            if (col.attachedRigidbody != null)
            {
                col.attachedRigidbody.AddExplosionForce(Force, transform.position, Force, UpwardsModifier);
                col.attachedRigidbody.AddTorque(Force, 0f, 0f);
                Player player = col.attachedRigidbody.GetComponent<Player>();
                if (player != null)
                    player.TakeDamage(Damage, name + "'s explosion");
                ExplodedRbList.Add(col.attachedRigidbody);
                //Debug.Log("Add explosion force to " + col.name, col);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, Radius);
        foreach (Rigidbody rb in ExplodedRbList)
        {
            Gizmos.DrawLine(transform.position, rb.position);
        }
    }
#endif
}
