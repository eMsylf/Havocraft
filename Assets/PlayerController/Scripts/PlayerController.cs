﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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
    public Rigidbody Rigidbody
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
    public ForceMode forceAddMethod = ForceMode.Acceleration;

    public bool ExternallyControlled = false;
    public bool ControlledByClient = false;

    bool boostActivated = false;
    [Tooltip("The value that needs to be exceeded before the boost is automatically applied")]
    [Range(0, 1)]
    public float BoostZoneTrigger = .8f;
    public float boostSpeedMultiplier = 2f;
    private void SetBoost(bool enabled)
    {
        boostActivated = enabled;
        if (boostActivated) OnBoostActivated.Invoke();
        else OnBoostStopped.Invoke();
    }
    public GameObject OnScreenControls;
    public GameObject OnScreenJoystick;


    public UnityEvent OnBoostActivated;
    public UnityEvent OnBoostStopped;

    [Header("Animation")]
    public bool Animation = true;
    [Tooltip("Min value = regular angle, Max value = boosting angle")]
    public Vector2 AnimationTiltAngle, AnimationRollAngle;
    [Range(0f, 1f)]
    public float AnimationRigidity = .5f;

    public bool ContinuouslyResetAnimationY = true;
    [Tooltip("Makes sure the weapons are aimed at the same angle relative to the world, so that the player doesn't shoot into the ground when tilted forward")]
    public bool GimbalWeapons = true;
    [Range(0f, 1f)]
    public float GimbalRigidity = 1f;
    //public bool PauseEditorUponFiring = false;
    Vector2 movementInput = new Vector2();
    Vector2 calculatedMovementInput
    {
        get
        {
            return new Vector2(movementInput.x * RotationSpeed, movementInput.y * Speed * (boostActivated ? boostSpeedMultiplier : 1f));
        }
    }

    public UnityEventVector2 onMovementInputChanged;

    private void Awake()
    {
        if (ExternallyControlled)
            return;
        //Controls.InGame.Movement.started += _ => Move(_.ReadValue<Vector2>());
        //Controls.InGame.Movement.performed += _ => Move(_.ReadValue<Vector2>());
        //Controls.InGame.Movement.canceled += _ => StopMove();
        Controls.InGame.Shoot.started += _ => SetShootingActive(_.ReadValueAsButton());
        Controls.InGame.Shoot.canceled += _ => SetShootingActive(false);
        Controls.InGame.Shoot.performed += _ => Shoot();
        Controls.InGame.Boost.performed += _ => SetBoost(true);
        Controls.InGame.Boost.canceled += _ => SetBoost(false);
    }

    void Update()
    {
        // Platform-dependent vector readings. Cannot be tested in editor when build platform is Android
#if UNITY_ANDROID
        Move(controls.InGame.Movement.ReadValue<Vector3>());
#else
        Move(controls.InGame.Movement.ReadValue<Vector2>());
#endif
        // Universal across all platforms
        // Checks if there is a sensor active. Allows Android phones without gyroscopes to use on-screen joystick instead
        //if (GravitySensor.current != null && GravitySensor.current.enabled)
        //{
        //    Move(controls.InGame.Movement.ReadValue<Vector3>());
        //}
        //else
        //{
        //    Move(controls.InGame.Movement.ReadValue<Vector2>());
        //}

        ShootContinuous();
    }

    private void FixedUpdate()
    {
        if (!Rigidbody.isKinematic)
        {
            ApplyForces(calculatedMovementInput);
        }
        
        if (Animation)
        {
            Vector3 animRotation = Body.localRotation.eulerAngles;
            animRotation.x = movementInput.y * (boostActivated ? AnimationTiltAngle.y : AnimationTiltAngle.x);
            animRotation.z = -movementInput.x * (boostActivated ? AnimationRollAngle.y : AnimationRollAngle.x);
            if (ContinuouslyResetAnimationY) animRotation.y = 0f;
            Body.localRotation = Quaternion.Lerp(Body.localRotation, Quaternion.Euler(animRotation), AnimationRigidity);

            if (GimbalWeapons)
            {
                foreach (Weapon weapon in GetComponentsInChildren<Weapon>())
                {
                    if (weapon.Barrel == null)
                        continue;
                    Quaternion storedRotation = weapon.Barrel.rotation;
                    weapon.Barrel.LookAt(weapon.Base.position + transform.forward);
                    Quaternion correctedRotation = Quaternion.Euler(weapon.barrelStartRotation.x, weapon.Barrel.rotation.eulerAngles.y, weapon.Barrel.rotation.eulerAngles.z);
                    weapon.Barrel.rotation = Quaternion.Lerp(storedRotation, correctedRotation, GimbalRigidity);
                }
            }
        }
    }

    void Move(Vector2 direction)
    {
        movementInput = Vector2.ClampMagnitude(direction, 1f);
        
#if UNITY_ANDROID
        SetBoost(Mathf.Abs(direction.y) > BoostZoneTrigger);
        if (boostActivated)
        {
            Debug.Log("Y input exceeds 1, activate boost");
        }
#endif

        if (Player.PlayerClientInterface != null)
        {
            Player.PlayerClientInterface.MovementChanged(calculatedMovementInput);
        }

        //Debug.Log("Move! " + direction);
        onMovementInputChanged.Invoke(calculatedMovementInput);
    }

    void StopMove()
    {
        //Debug.Log("Stop moving");
        Move(Vector2.zero);
    }

    public void ApplyForces(Vector2 input)
    {
        Rigidbody.AddRelativeForce(0f, 0f, input.y, forceAddMethod);
        Rigidbody.AddTorque(0f, input.x, 0f, forceAddMethod);
    }

    void Shoot()
    {
        if (ControlledByClient)
            return;
        Debug.Log("Shoot", this);
        foreach (Weapon weapon in GetComponentsInChildren<Weapon>())
        {
            weapon.Fire();
        }
//#if UNITY_EDITOR
//        if (PauseEditorUponFiring) UnityEditor.EditorApplication.isPaused = true;
//#endif
    }

    Player player;
    public Player Player
    {
        get
        {
            if (player == null)
            {
                player = GetComponent<Player>();
                if (player == null)
                    Debug.LogError("Player Controller has no Player component assigned");
            }
            return player;
        }
    }

    bool shooting = false;
    public void SetShootingActive(bool _shooting)
    {
        if (Player.PlayerClientInterface == null)
        {
            Debug.Log("Set shooting active: " + _shooting);
            shooting = _shooting;
            if (shooting)
                Shoot();
        }
        else
        {
            Debug.Log("Set shooting active: " + _shooting);
            Player.PlayerClientInterface.ShootingChanged(_shooting);
        }
        OnShootingChanged.Invoke(_shooting);
    }
    public UnityEventBool OnShootingChanged;

    void ShootContinuous()
    {
        if (!shooting)
            return;
        foreach (Weapon weapon in GetComponentsInChildren<Weapon>())
        {
            if (weapon.ContinuousFire)
            {
                weapon.Fire();
            }
        }
//#if UNITY_EDITOR
//        if (PauseEditorUponFiring) UnityEditor.EditorApplication.isPaused = true;
//#endif
    }

    private void OnEnable()
    {
        Debug.Log("Enable " + name, gameObject);

#if UNITY_ANDROID
        if (GravitySensor.current != null)
        {
            InputSystem.EnableDevice(GravitySensor.current);
            Debug.Log("Enabled gravity sensor");
            GravitySensor.current.samplingFrequency = 16;
        }
        else
        {
            // Activates an on-screen joystick if no gravity sensor is present
            if (OnScreenJoystick != null)
                OnScreenJoystick.SetActive(true);
            Debug.LogError("No gravity sensor");
        }
#else
        if (OnScreenControls != null)
            OnScreenControls.SetActive(false);
#endif

        Controls.Enable();
    }

    private void OnDisable()
    {
        Debug.Log("Disable " + name, gameObject);
#if UNITY_ANDROID
        if (GravitySensor.current != null)
        {
            InputSystem.DisableDevice(GravitySensor.current);
            Debug.Log("Disabled gravity sensor");
        }
        else
        {
            if (OnScreenJoystick != null)
                OnScreenJoystick.SetActive(false);
            Debug.LogError("No gravity sensor");
        }
#endif

        Controls.Disable();
    }
}
