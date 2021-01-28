using BobJeltes.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
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

    public Slider Health;
    public ValueText ScoreValue;
    public PlayerController PlayerController;
    public List<GameObject> HoverJets;
    public List<GameObject> DeathEffectObjects;
    public EndGameScreen endGameScreen;

    bool disabling = false;

    private void OnEnable()
    {
        disabling = false;
        Debug.Log("Enable " + name, gameObject);
    }

    private void OnDisable()
    {
        disabling = true;
        Debug.Log("Disable " + name, gameObject);
    }


    public void TakeDamage(float damage)
    {
        Health.value -= damage;
        Debug.Log(name + " takes " + damage + " damage", this);
    }

    public void TakeDamage(float damage, string damager)
    {
        Debug.Log(damager + " damages " + name);
        TakeDamage(damage);
    }

    public UnityEvent OnDeath;
    public void Die()
    {
        ExplodeViolently();
        TurnOffHoverjets();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDeath(this);
        }
        OnDeath.Invoke();
        if (!disabling)
            enabled = false;

    }

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
