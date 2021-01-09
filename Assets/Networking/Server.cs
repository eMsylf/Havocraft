using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkManager))]
public class Server : MonoBehaviour
{
    NetworkManager networkManager;
    public NetworkManager NetworkManager
    {
        get
        {
            if (networkManager == null)
            {
                networkManager = GetComponent<NetworkManager>();
                if (networkManager == null)
                    Debug.LogError("Network manager is missing!");
            }
            return networkManager;
        }
    }
    void Start()
    {
        //NetworkManager.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
