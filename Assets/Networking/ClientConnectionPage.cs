using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientConnectionPage : MonoBehaviour
{
    public ClientBehaviour client;

    public void SetIP(string ip)
    {
        client.IPAddress = ip;
    }

    public void SetPort(string port)
    {
        bool succesfulParse = ushort.TryParse(port, out ushort result);
        if (succesfulParse)
        {
            client.port = result;
        }
        else
        {
            Debug.LogError("Invalid port entry");
        }
    }
}
