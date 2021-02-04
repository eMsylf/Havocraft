using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes
{
    [System.Serializable]
    public class Cooldown
    {
        [Min(0)]
        public float Time = 1f;
        [SerializeField]
        private bool isActive = false;
        public bool IsActive { get => isActive; private set { isActive = value; } }

        public IEnumerator Start()
        {
            if (Time <= 0)
                yield break;
            IsActive = true;
            yield return new WaitForSeconds(Time);
            IsActive = false;
        }
    }
}