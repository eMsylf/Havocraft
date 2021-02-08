using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDamage : MonoBehaviour
{
    public float Damage = 1f;
    public LayerMask AffectedLayers = new LayerMask();
    private void OnCollisionEnter(Collision collision)
    {
        if (AffectedLayers != (AffectedLayers | 1 << collision.gameObject.layer))
        {
            Debug.Log("Hit object not in affected layer");
            return;
        }

        Health healthComponent = collision.collider.GetComponentInParent<Health>();
        if (healthComponent != null)
        {
            healthComponent.UpdateValue(healthComponent.value - Damage);
        }
    }
}
