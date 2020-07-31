using BJ;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject Ammunition;
    public float Firepower = 10f;
    [Tooltip("The end of the barrel of the weapon")]
    public Transform Barrel;
    public Transform Muzzle;

    public Cooldown Cooldown;

    private void Start()
    {
        Cooldown.Stop();
    }

    private void Update()
    {
        //if (Cooldown.Active)
        //    Cooldown.Update(Time.deltaTime);
    }

    public virtual void Fire()
    {
        if (Cooldown.Check())
        {
            Debug.Log(name + " is on cooldown. Time remaining: " + Cooldown.Current, this);
            return;
        }
        Debug.Log("Fire " + name);
        if (Ammunition != null)
        {
            // Instantiate
            GameObject projectile = Instantiate(Ammunition, Muzzle.position, Muzzle.rotation);

            // Add force
            projectile.GetComponent<Rigidbody>()?.AddForce(GetComponentInParent<Rigidbody>().velocity + Muzzle.forward * Firepower, ForceMode.Impulse);

            // Add an owner to the projectile
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            if (projectileComponent == null)
            {
                projectileComponent = projectile.AddComponent<Projectile>();
            }
            projectileComponent.Owner = GetComponentInParent<PlayerController>();

            Cooldown.Start();
        }
    }
}
