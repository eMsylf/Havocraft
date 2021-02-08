using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float value = 100f;
    public Slider slider;
    public UnityEvent OnHealthDepleted;

    public void UpdateValue(float val)
    {
        value = val;
        if (slider == null)
            Debug.LogError("Health component has no slider", this);
        else
            slider.value = value;
        Debug.Log("Value: " + value);
        if (value <= 0f)
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
