using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    protected override void Impact(Collision collisionData)
    {
        base.Impact(collisionData);
        if (DisappearUponImpact)
        {
            gameObject.SetActive(false);
        }
    }
}
