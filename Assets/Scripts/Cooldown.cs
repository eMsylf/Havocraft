using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes
{
    [System.Serializable]
    public class Cooldown
    {
        public float Duration = 1f;
        public bool Active = false;

        public IEnumerator Start()
        {
            Active = true;
            yield return new WaitForSeconds(Duration);
            Active = false;
        }
    }
}