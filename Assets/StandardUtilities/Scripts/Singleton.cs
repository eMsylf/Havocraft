using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.StandardUtilities
{

    /// <summary>
    /// Inherit from this base class to create a singleton.
    /// e.g. public class MyClassName : Singleton<MyClassName> {}
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Check to see if we're about to be destroyed.
        private static bool m_ShuttingDown = false;
        private static object m_Lock = new object();
        private static T m_Instance;
        public static bool HasActiveInstance()
        {
            if (m_Instance == null)
                m_Instance = FindObjectOfType<T>();
            return m_Instance != null;
        }

        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (m_ShuttingDown)
                {
                    T newInstance = FindObjectOfType<T>();
                    if (newInstance == null) {
                        Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                            "' already destroyed. Returning null.");
                        return null;
                    }
                    else
                    {
                        Debug.Log("New instance of singleton found in active scene. Returning new: " + newInstance.name, newInstance);
                        m_ShuttingDown = false;
                        return newInstance;
                    }
                }

                lock (m_Lock)
                {
                    if (m_Instance == null)
                    {
                        // Search for existing instance.
                        m_Instance = (T)FindObjectOfType(typeof(T));

                        // Create new instance if one doesn't already exist.
                        if (m_Instance == null)
                        {
                            // Need to create a new GameObject to attach the singleton to.
                            var singletonObject = new GameObject();
                            m_Instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";

                            // Make instance persistent.
                            DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return m_Instance;
                }
            }
            set => m_Instance = value;
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Quitting");
            m_ShuttingDown = true;
        }

        private void OnDestroy()
        {
            Debug.Log("Destroyed instance: " + name);
            m_ShuttingDown = true;
        }
    }
}