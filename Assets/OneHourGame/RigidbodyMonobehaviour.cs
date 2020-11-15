using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMonobehaviour : MonoBehaviour
{
    private Rigidbody rb;
    public Rigidbody Rigidbody
    {
        get
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
            return rb;
        }
    }
}
