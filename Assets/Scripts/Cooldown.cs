using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BJ
{
    [System.Serializable]
    public class Cooldown
    {
        public float Duration = 1f;
        public float Current = 0f;
        public bool Active = false;
        private float startTime = -Mathf.Infinity;

        public Cooldown(float duration)
        {
            Duration = duration;
        }
        /// <summary>
        /// Checks if the cooldown is complete
        /// </summary>
        /// <returns>True if the cooldown is over</returns>
        public bool Check()
        {
            //return Current <= Seconds;
            Current = Time.time - startTime;
            if (Current < 0)
            {
                Stop();
            }
            return Current < 0;
        }

        public void Update(float deltaTime)
        {
            Advance(deltaTime);
            if (Complete)
            {
                Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns>Whether cooldown has completed</returns>
        public void Advance(float deltaTime)
        {
            if (Active)
                Current -= deltaTime;
            else
                Debug.LogWarning("Can't advance while cooldown is paused or stopped");
            return;
        }

        public bool Complete
        {
            get { return Current <= 0f; }
        }

        public void Start()
        {
            Active = true;
            Current = Duration;
            startTime = Time.time;
            Debug.Log("Start cooldown");
        }

        public void Pause()
        {
            Active = false;
            Debug.Log("Pause cooldown");
        }

        public void Stop()
        {
            Active = false;
            Current = Duration;
            Debug.Log("Stop cooldown");
        }
    }

    public enum CountType
    {
        Up,
        Down
    }
}