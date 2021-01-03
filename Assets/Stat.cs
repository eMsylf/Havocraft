using BobJeltes;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    [System.Serializable]
    public class Stat : MonoBehaviour
    {
        public float m_maxValue = 4;
        public float m_value;
        [Header("Recovery")]
        [Tooltip("The time it takes for a point to be recovered..")]
        public float recoveryTime = 1f;
        public float recoveryWindup = 1f;
        private float windup;
        private float recovery;
        public bool allowRecovery = false;
        [Header("Other")]
        [Tooltip("When enabled, this statistic is set to its maximum automatically on startup.")]
        public bool syncValueToMax = true;

        public float MaxValue
        {
            get => m_maxValue;
            set
            {
                m_maxValue = value;
            }
        }

        public float Value
        {
            get => m_value;
            set
            {
                m_value = value;
            }
        }

        public UnityEventFloat OnValueChanged;
        public UnityEvent OnDepleted;

        public float Get()
        {
            return m_value;
        }

        public void SetCurrent(float _value)
        {
            if (m_value == _value)
                return;
            m_value = _value;
            Debug.Log("Set " + name + " to " + _value + " points", this);
            OnValueChanged.Invoke(_value);
        }

        public void Use(float amount)
        {
            if (m_value - amount >= 0)
            {
                SetCurrent(m_value - amount);
            }

            if (m_value <= 0 && OnDepleted != null)
                OnDepleted.Invoke();
        }

        private void Start()
        {
            if (syncValueToMax)
            {
                Value = MaxValue;
            }
        }

        private void Update()
        {
            ManageRecharge();
        }

        void ManageRecharge()
        {
            if (!allowRecovery)
                return;
            if (m_value < m_maxValue)
            {
                if (windup < recoveryWindup)
                {
                    windup += Time.deltaTime;
                }
                else
                {
                    if (recovery < recoveryTime)
                    {
                        recovery += Time.deltaTime;
                    }
                    else
                    {
                        SetCurrent(m_value + 1);
                        recovery = 0f;
                    }
                }
            }
        }
    }
}
