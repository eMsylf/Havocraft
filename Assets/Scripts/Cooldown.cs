using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes
{
    [System.Serializable]
    public class Cooldown
    {
        public float Time = 1f;
        public bool Active = false;

        public IEnumerator Start()
        {
            Active = true;
            yield return new WaitForSeconds(Time);
            Active = false;
        }
    }
}