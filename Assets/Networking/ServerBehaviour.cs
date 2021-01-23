﻿using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine.Events;
//using UnityEngine.Networking;

public class ServerBehaviour : MonoBehaviour
{
    public bool AutoStart = true;
    //public string IPAddressInput = "";
    [SerializeField]
    private string ipAddress = "";
    public string IPAddress
    {
        get => ipAddress;
        set
        {
            ipAddress = value;
            OnIPSet.Invoke(value);
        }
    }
    public bool ClearIPOnStop = true;
    public UnityEventString OnIPSet;

    [SerializeField]
    private ushort port = 9000;
    public ushort Port
    {
        get => port;
        set
        {
            port = value;
            OnPortSet.Invoke(value.ToString());
        }
    }
    public bool ClearPortOnStop = true;
    public UnityEventString OnPortSet;

    public NetworkDriver m_Driver;
    private NativeList<NetworkConnection> m_Connections;
    public UnityEventString OnConnectionCountChanged;

    public UnityEvent OnServerStart;
    public UnityEvent OnServerStop;

    public uint messageValue;

    private void Start()
    {
        if (AutoStart)
            StartServer();
    }

    private void OnDestroy()
    {
        StopServer();
    }

    private void Update()
    {
        if (!m_Driver.IsCreated)
            return;
        m_Driver.ScheduleUpdate().Complete();

        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                OnConnectionCountChanged.Invoke(m_Connections.Length.ToString());
                i--;
            }
        }

        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default)
        {
            m_Connections.Add(c);

            OnConnectionCountChanged.Invoke(m_Connections.Length.ToString());
            Debug.Log("Accepted a connection");
        }

        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                continue;
            }
            NetworkEvent.Type cmd;
            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {

                    if (!stream.IsCreated)
                    {
                        Debug.Log("Received ping");
                    }
                    else
                    {
                        Debug.Log("Stream length: " + stream.Length);
                        uint number = stream.ReadUInt();
                        Debug.Log("Got " + number + " from the client. Adding it to the total of " + messageValue);

                        messageValue += number;

                        Debug.Log("Sending " + messageValue);

                        var writer = m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i]);
                        writer.WriteUInt(messageValue);
                        m_Driver.EndSend(writer);
                    }
                    
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnect from server");
                    m_Connections[i] = default;
                }
            }
        }
    }

    public float PingInterval = 1f;
    private float timeSinceLastPing = 0f;

    public void Ping()
    {
        // Check time
        if (timeSinceLastPing < PingInterval)
        {
            timeSinceLastPing += Time.deltaTime;
            return;
        }
        // Ping
        for (int i = 0; i < m_Connections.Length; i++)
        {
            NetworkConnection connection = m_Connections[i];
            if (!connection.IsCreated)
                return;
            if (connection.GetState(m_Driver) != NetworkConnection.State.Connected)
                return;
            Debug.Log("Ping");
            var writer = m_Driver.BeginSend(connection);
            m_Driver.EndSend(writer);
        }
        timeSinceLastPing = 0f;
    }

    public void StartServer()
    {
        if (m_Driver.IsCreated)
        {
            Debug.Log("Server already started");
            return;
        }

        //Debug.Log("Start server");
        m_Driver = NetworkDriver.Create();
        //var endpoint = NetworkEndPoint.AnyIpv4;

        Port = port;

        //endpoint.Port = port;
        if (m_Driver.Bind(NetworkEndPoint.Parse(IPManager.GetLocalIPAddress(), Port)) != 0)
            Debug.Log("Failed to bind to port " + Port);
        else m_Driver.Listen();

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        
        IPAddress = IPManager.GetLocalIPAddress();
        Debug.Log("Started server at " + IPAddress);
        OnServerStart.Invoke();
    }

    public void StopServer()
    {
        for (int i = 0; i < m_Connections.Length; i++)
        {
            var writer = m_Driver.BeginSend(m_Connections[i]);
            //writer.
            m_Connections[i].Disconnect(m_Driver);
        }
        if (m_Driver.IsCreated)
            m_Driver.Dispose();
        if (m_Connections.IsCreated)
            m_Connections.Dispose();

        if (ClearIPOnStop) IPAddress = "0.0.0.0";
        if (ClearPortOnStop) Port = default;
        Debug.Log("Stopped server");
        OnServerStop.Invoke();
    }
}
