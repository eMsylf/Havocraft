using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public UnityEvent OnHealthDepleted;

    public void ProcessValue(float val)
    {
        Debug.Log("Value: " + val);
        if (Mathf.Approximately(val, 0f))
        {
            HealthDepleted();
        }
    }

    public void HealthDepleted()
    {
        Debug.Log("Health depleted");
        OnHealthDepleted.Invoke();
    }
}
