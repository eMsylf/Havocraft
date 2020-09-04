using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.StandardUtilities
{
    public class Spinner : MonoBehaviour
    {
        public float Speed = 1f;
        void Update()
        {
            transform.Rotate(0f, 0f, Speed);
        }
    }
}
