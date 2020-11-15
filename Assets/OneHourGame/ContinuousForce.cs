using UnityEngine;

public class ContinuousForce : RigidbodyMonobehaviour
{
    public Vector3 Direction = Vector3.forward;
    public ForceMode forceMode = ForceMode.Force;
    public enum Space
    {
        Local,
        World
    }
    public Space space;

    private void FixedUpdate()
    {
        switch (space)
        {
            case Space.Local:
                Rigidbody.AddRelativeForce(Direction, forceMode);
                break;
            case Space.World:
                Rigidbody.AddForce(Direction, forceMode);
                break;
        }
    }
}
