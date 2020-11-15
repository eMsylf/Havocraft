using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : RigidbodyMonobehaviour
{
    public Vector2 inputMultiplier = Vector2.one;
    public ForceMode forceMode = ForceMode.Force;

    public enum Space
    {
        World,
        Local
    }
    public Space space = Space.Local;
    //[Tooltip("If Local is selected, drag another object in here to make movement relative to it.")]
    //public Transform OverrideLocal;

    private Vector3 movement;
    private void FixedUpdate()
    {
        switch (space)
        {
            case Space.World:
                Rigidbody.AddForce(movement, forceMode);
                break;
            case Space.Local:
                //if (OverrideLocal != null)
                //{
                //    movement = ObjectRelative(movement, OverrideLocal);
                //    Rigidbody.AddForce(movement, forceMode);
                //}
                //else
                //{
                //}
                Rigidbody.AddRelativeForce(movement, forceMode);
                break;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        Debug.Log("Move", this);
        Vector2 input = context.ReadValue<Vector2>();
        input *= inputMultiplier;
        movement = input;
    }

    //private Vector3 ObjectRelative(Vector3 inputVector, Transform tr)
    //{
    //    Vector3 trForward = Vector3.Scale(tr.forward, new Vector3(1, 0, 1)).normalized;
    //    Vector3 trRight = tr.right;
    //    Vector3 objectRelativeMovement = trForward * inputVector.z + trRight * inputVector.x;
    //    return objectRelativeMovement;
    //}
}
