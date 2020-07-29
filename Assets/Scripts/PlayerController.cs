using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
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
            }
            return rigidbody;
        }
    }

    public Transform Body;

    public float Speed = 3f;
    public float RotationSpeed = 1f;

    private void Awake()
    {
        //Controls.InGame.Movement.performed += _ => Move(_.ReadValue<Vector2>());
        //Controls.InGame.Movement.canceled += _ => StopMove();
        Controls.InGame.Shoot.performed += _ => Shoot();
        Controls.InGame.Boost.performed += _ => Boost(true);
        Controls.InGame.Boost.canceled += _ => Boost(false);
    }

    bool boostActivated = false;
    public float boostSpeedMultiplier = 2f;
    private void Boost(bool enabled)
    {
        boostActivated = enabled;
    }

    void Start()
    {
    }

    public bool Animation = true;
    [Range(0f, 90f)]
    public float AnimationTiltAngle = 10f;
    [Range(0f, 1f)]
    public float AnimationRigidity = .5f;

    void Update()
    {
        movementInput = controls.InGame.Movement.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Rigidbody.AddRelativeForce(0f, 0f, movementInput.y * Speed * (boostActivated?boostSpeedMultiplier:1f));
        Rigidbody.AddTorque(0f, movementInput.x * RotationSpeed, 0f);
        if (Animation)
        {
            Vector3 rotation = Body.localRotation.eulerAngles;
            rotation.x = movementInput.y * AnimationTiltAngle;
            Body.localRotation = Quaternion.Lerp(Body.localRotation, Quaternion.Euler(rotation), AnimationRigidity);
        }
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
            TurnOffHoverjets();
        }
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
        foreach (GameObject hoverJet in HoverJets)
        {
            hoverJet.SetActive(false);
        }
    }
}
