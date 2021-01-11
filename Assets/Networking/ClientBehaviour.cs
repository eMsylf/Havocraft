using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class ClientBehaviour : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public bool AutoConnect = true;
    public float connectionReportInterval = 1f;
    public float connectionTimeOut = 10f;
    public bool AutoDisconnect = true;
    public ushort port = 9000;
    //public bool Done;
    [HideInInspector]
    public uint value = 1;

    private void Start()
    {
        if (AutoConnect)
            ConnectToServer(false);
    }

    private void OnDestroy()
    {
        if (m_Connection.IsCreated)
            Disconnect();
        if (m_Driver.IsCreated)
            m_Driver.Dispose();
    }

    void Update()
    {
        if (!m_Driver.IsCreated)
            return;
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd = m_Connection.PopEvent(m_Driver, out stream); 
        
        //Debug.Log("Network Event Type: " + cmd.ToString());
        switch (cmd)
        {
            case NetworkEvent.Type.Empty:
                break;
            case NetworkEvent.Type.Data:
                uint newValue = stream.ReadUInt();
                Debug.Log(name + " got the value = " + newValue + " back from the server");
                //Done = true;
                if (AutoDisconnect)
                {
                    Disconnect();
                }
                break;
            case NetworkEvent.Type.Connect:
                Debug.Log(name + " is now connected to the server", gameObject);
                
                break;
            case NetworkEvent.Type.Disconnect:
                Debug.Log("Client got disconnected from server", this);
                m_Connection = default;
                break;
            default:
                break;
        }
    }

    public void ConnectToServer(bool forceReconnection)
    {
        if (forceReconnection && m_Connection.IsCreated) Disconnect();
        if (!m_Driver.IsCreated) m_Driver = NetworkDriver.Create();
        m_Connection = default;

        var endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = port;
        m_Connection = m_Driver.Connect(endpoint);

        ReportConnectionState();
        StartCoroutine(ConnectStatus(connectionReportInterval));
    }

    public IEnumerator ConnectStatus(float interval)
    {
        if (!m_Driver.IsCreated) yield break;
        if (!m_Connection.IsCreated) yield break;

        float connectionTime = 0f;

        while (m_Connection.GetState(m_Driver) == NetworkConnection.State.Connecting)
        {
            Debug.Log("Connecting...");
            yield return new WaitForSeconds(interval);
            connectionTime += interval;
            if (connectionTime >= connectionTimeOut)
            {
                Debug.LogError("Connection timed out.", this);
                Disconnect();
                break;
            }
        }
    }

    public void ReportConnectionState()
    {
        if (m_Connection.IsCreated) Debug.Log("Connection state: " + m_Connection.GetState(m_Driver).ToString());
        else Debug.LogError(name + "has no active connection");
    }

    public void SendValueToServer()
    {
        var writer = m_Driver.BeginSend(m_Connection);
        writer.WriteUInt(value);
        m_Driver.EndSend(writer);
    }

    public void Disconnect()
    {
        if (!m_Connection.IsCreated)
        {
            Debug.LogError("No active connection");
            return;
        }
        m_Connection.Disconnect(m_Driver);
        m_Connection = default;
        m_Driver.Dispose();
        Debug.Log(name + " disconnected from the server", this);
    }
}
