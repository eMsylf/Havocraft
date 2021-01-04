using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Player : MonoBehaviour
{
    new Rigidbody rigidbody;
    Rigidbody Rigidbody
    {
        get
        {
            if (rigidbody == null)
            {
                rigidbody = GetComponent<Rigidbody>();
            }
            return rigidbody;
        }
    }

    private void OnEnable()
    {
        Debug.Log("Enable " + name, gameObject);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDeath(this);
        }
        Debug.Log("Disable " + name, gameObject);
    }

    public Slider Health;

    public void TakeDamage(float damage)
    {
        Health.value = Health.value - damage;
        Debug.Log(name + " takes " + damage + " damage", this);
    }

    public void TakeDamage(float damage, string damager)
    {
        Debug.Log(damager + " damages " + name);
        TakeDamage(damage);
    }

    public void Die()
    {
        ExplodeViolently();
        TurnOffHoverjets();
    }

    public List<GameObject> HoverJets;
    public List<GameObject> DeathEffectObjects;

    private void ExplodeViolently()
    {
        Rigidbody.constraints = RigidbodyConstraints.None;
        Rigidbody.useGravity = true;
        if (DeathEffectObjects != null)
        {
            foreach (GameObject obj in DeathEffectObjects)
            {
                obj.SetActive(true);
                VisualEffect vfx = obj.GetComponent<VisualEffect>();
                // Give the visual effect additional velocity
                if (vfx != null)
                {
                    if (vfx.HasVector3("Additional Velocity"))
                        vfx.SetVector3("Additional Velocity", Rigidbody.velocity);
                }
            }
        }
        enabled = false;
    }

    void TurnOffHoverjets()
    {
        ConstantForce force = GetComponent<ConstantForce>();
        if (force != null) force.enabled = false;
        foreach (GameObject hoverJet in HoverJets)
        {
            hoverJet.SetActive(false);
        }
    }
}
