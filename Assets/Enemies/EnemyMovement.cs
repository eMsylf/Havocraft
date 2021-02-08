using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : RigidbodyMonobehaviour
{
    public Player targetPlayer;
    public float Speed = 1f;
    public ForceMode SpeedType = ForceMode.Acceleration;

    private void Awake()
    {
        if (targetPlayer == null)
            targetPlayer = FindObjectOfType<Player>();
    }

    private void FixedUpdate()
    {
        if (targetPlayer == null)
            return;

        Vector3 movementVector = Vector3.Normalize(targetPlayer.transform.position - Rigidbody.position);
        Rigidbody.AddForce(movementVector * Speed, SpeedType);
    }
}
