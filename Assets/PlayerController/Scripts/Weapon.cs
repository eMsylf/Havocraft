using BobJeltes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject Ammunition;
    [Tooltip("The speed of the projectile")]
    public float ProjectileSpeed = 10f;
    [Tooltip("The end of the barrel of the weapon")]
    public Transform Base;
    public Transform Barrel;
    public Transform Muzzle;

    public Cooldown Cooldown;
    [HideInInspector]
    public Vector3 weaponStartRotation, barrelStartRotation;

    [Tooltip("When enabled, holding down the FIRE button will cause the weapon to keep shooting as long as the FIRE button is held down.")]
    public bool ContinuousFire = false;

    private void Start()
    {
        weaponStartRotation = Base.rotation.eulerAngles;
        barrelStartRotation = Barrel.rotation.eulerAngles;
    }

    private void Update()
    {
    }

    public float ParentVelocityInfluence = 1f;

    public virtual void Fire()
    {
        if (Cooldown.Active)
        {
            //Debug.Log(name + " is on cooldown.", this);
            return;
        }
        //Debug.Log("Fire " + name);
        if (Ammunition != null)
        {
            // Instantiate
            GameObject projectile = Instantiate(Ammunition, Muzzle.position, Muzzle.rotation);

            // Add force
            projectile.GetComponent<Rigidbody>()?.AddForce(GetComponentInParent<Rigidbody>().velocity * ParentVelocityInfluence + Muzzle.forward * ProjectileSpeed, ForceMode.Impulse);

            // Add an owner to the projectile
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            if (projectileComponent == null)
            {
                projectileComponent = projectile.AddComponent<Projectile>();
            }
            projectileComponent.Owner = GetComponentInParent<Player>();
            if (ServerBehaviour.HasActiveInstance())
            {
                ServerBehaviour.Instance.projectiles.Add(projectileComponent);
            }

            StartCoroutine(Cooldown.Start());
        }
    }
}
