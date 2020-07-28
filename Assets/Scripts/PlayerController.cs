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
        controls.InGame.Movement.canceled += _ => StopMove();
    }

    void Start()
    {
        
    }

    void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        Rigidbody.AddRelativeForce(0f, 0f, movementInput.y * Speed);
        Rigidbody.AddTorque(0f, movementInput.x * RotationSpeed, 0f);
    }

    void GetInput()
    {
        //if (Controls.InGame.Movement.phase == InputActionPhase.Started)
        //{
        //    Debug.Log("Started");
        //}
    }

    Vector2 movementInput;

    void Move(Vector2 direction)
    {
        Debug.Log("Move! " + direction);
        //Controls.InGame.Movement.
        movementInput = direction;
    }

    void StopMove()
    {
        Debug.Log("Stop moving");
        movementInput = Vector2.zero;
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
}
