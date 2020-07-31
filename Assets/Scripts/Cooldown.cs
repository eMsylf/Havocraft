using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BJ
{
    [System.Serializable]
    public class Cooldown
    {
        public float Seconds = 1f;
        public float Current = 0f;
        public bool Active = false;

        public Cooldown(float duration)
        {
            Seconds = duration;
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
            Current = Seconds;
        }

        public void Pause()
        {
            Active = false;
        }

        public void Stop()
        {
            Active = false;
        }
    }

    public enum CountType
    {
        Up,
        Down
    }
}