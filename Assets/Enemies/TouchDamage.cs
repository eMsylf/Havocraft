using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDamage : MonoBehaviour
{
    public float Damage = 1f;
    private void OnCollisionEnter(Collision collision)
    {
        Health healthComponent = collision.collider.GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.UpdateValue(healthComponent.value - Damage);
        }
    }
}
