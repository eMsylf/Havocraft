using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    private GameControls controls;
    private GameControls Controls
    {
        get
        {
            if (controls == null)
            {
                controls = new GameControls();
            }
            return controls;
        }
    }

    new Rigidbody rigidbody;
    Rigidbody Rigidbody
    {
        get
        {
            if (rigidbody == null)
            {
                rigidbody = GetComponent<Rigidbody>();
                if (rigidbody == null)
                {
                    rigidbody = gameObject.AddComponent<Rigidbody>();
                }
            }
            return rigidbody;
        }
    }

    public float Speed = 3f;
    public float RotationSpeed = 1f;

    private void Awake()
    {
        Controls.InGame.Movement.performed += _ => Move(_.ReadValue<Vector2>());
        Controls.InGame.Movement.canceled += _ => StopMove();
        Controls.InGame.Shoot.performed += _ => Shoot();
        Controls.InGame.Boost.performed += _ => Boost(true);
        Controls.InGame.Boost.canceled += _ => Boost(false);
    }

    bool boostActivated = false;
    float boostSpeedMultiplier = 2f;
    private void Boost(bool enabled)
    {
        boostActivated = enabled;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        Rigidbody.AddRelativeForce(0f, 0f, movementInput.y * Speed * (boostActivated?boostSpeedMultiplier:1f));
        Rigidbody.AddTorque(0f, movementInput.x * RotationSpeed, 0f);
    }

    Vector2 movementInput;

    void Move(Vector2 direction)
    {
        Debug.Log("Move! " + direction);
        movementInput = direction;
    }

    void StopMove()
    {
        Debug.Log("Stop moving");
        movementInput = Vector2.zero;
    }

    void Shoot()
    {
        Debug.Log("Shoot");
        foreach (Weapon weapon in GetComponentsInChildren<Weapon>())
        {
            weapon.Fire();
        }
    }

    private void OnEnable()
    {
        Debug.Log("Enable " + name, gameObject);
        Controls.Enable();
    }

    private void OnDisable()
    {
        Debug.Log("Disable " + name, gameObject);
        Controls.Disable();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Projectile projectile = collision.collider.GetComponent<Projectile>();
        if (projectile != null)
        {
            ExplodeViolently();
        }
    }

    private void ExplodeViolently()
    {

    }
}
