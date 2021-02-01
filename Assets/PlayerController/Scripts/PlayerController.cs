using UnityEngine;
using UnityEngine.Events;
using GD.MinMaxSlider;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("If in the scene that's hosted by the server")]
    public bool ServerControlled = false;
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

    private void Awake()
    {
        if (ServerControlled)
            return;
        Controls.InGame.Movement.started += _ => Move(_.ReadValue<Vector2>());
        Controls.InGame.Movement.performed += _ => Move(_.ReadValue<Vector2>());
        Controls.InGame.Movement.canceled += _ => StopMove();
        Controls.InGame.Shoot.started += _ => SetShootingActive(_.ReadValueAsButton());
        Controls.InGame.Shoot.canceled += _ => SetShootingActive(false);
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

    public bool Animation = true;
    [Tooltip("Min value = regular tilt angle, Max value = boosting tilt angle")]
    [Header("Min = unboosted, Max = boosted")]
    [MinMaxSlider(0f, 90f)]
    public Vector2 AnimationTiltAngle;
    [Tooltip("Min value = regular tilt angle, Max value = boosting tilt angle")]
    [MinMaxSlider(0f, 90f)]
    public Vector2 AnimationRollAngle;
    [Range(0f, 1f)]
    public float AnimationRigidity = .5f;

    void Update()
    {
        // The below line is an alternative for the Move() and StopMove() functions
        //movementInput = controls.InGame.Movement.ReadValue<Vector2>();

        ShootContinuous();
    }
    public bool ContinuouslyResetAnimationY = true;
    [Tooltip("Makes sure the weapons are aimed at the same angle relative to the world, so that the player doesn't shoot into the ground when tilted forward")]
    public bool GimbalWeapons = true;
    [Range(0f, 1f)]
    public float GimbalRigidity = 1f;
    public bool PauseEditorUponFiring = false;
    Vector2 movementInput = new Vector2();
    Vector2 calculatedMovementInput
    {
        get
        {
            return new Vector2(movementInput.x * RotationSpeed, movementInput.y * Speed * (boostActivated ? boostSpeedMultiplier : 1f));
        }
    }

    public UnityEventVector2 onMovementInputChanged;

    void Move(Vector2 direction)
    {
        //Debug.Log("Move! " + direction);
        movementInput = direction;
        onMovementInputChanged.Invoke(calculatedMovementInput);
    }

    void StopMove()
    {
        //Debug.Log("Stop moving");
        Move(Vector2.zero);
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

    public void ApplyForces(Vector2 input)
    {
        Rigidbody.AddRelativeForce(0f, 0f, input.y, forceAddMethod);
        Rigidbody.AddTorque(0f, input.x, 0f, forceAddMethod);
    }

    void Shoot()
    {
        //Debug.Log("Shoot");
        foreach (Weapon weapon in GetComponentsInChildren<Weapon>())
        {
            weapon.Fire();
        }
#if UNITY_EDITOR
        if (PauseEditorUponFiring) UnityEditor.EditorApplication.isPaused = true;
#endif
    }


    bool shooting = false;
    public void SetShootingActive(bool _shooting)
    {
        shooting = _shooting;
        if (shooting)
            Shoot();
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
#if UNITY_EDITOR
        if (PauseEditorUponFiring) UnityEditor.EditorApplication.isPaused = true;
#endif
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
