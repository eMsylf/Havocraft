using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;

public class ServerBehaviour : MonoBehaviour
{
    public bool AutoStart = true;
    //public string IPAddressInput = "";
    public string IPAddressOutput = "";
    [SerializeField]
    private ushort port = 9000;
    public NetworkDriver m_Driver;
    private NativeList<NetworkConnection> m_Connections;

    public uint value;

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
                i--;
            }
        }

        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default)
        {
            m_Connections.Add(c);
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
                        Debug.Log("Got " + number + " from the client. Adding it to the total of " + value);

                        value += number;

                        Debug.Log("Sending " + value);

                        var writer = m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i]);
                        writer.WriteUInt(value);
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

    public void StartServer()
    {
        if (m_Driver.IsCreated)
        {
            Debug.Log("Server already started");
            return;
        }

        Debug.Log("Start server");
        m_Driver = NetworkDriver.Create();
        //var endpoint = NetworkEndPoint.AnyIpv4;
        
        //endpoint.Port = port;
        if (m_Driver.Bind(NetworkEndPoint.Parse(IPManager.GetLocalIPAddress(), port)) != 0)
            Debug.Log("Failed to bind to port " + port);
        else m_Driver.Listen();

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        
        IPAddressOutput = IPManager.GetLocalIPAddress();
    }

    public void StopServer()
    {
        if (m_Driver.IsCreated)
            m_Driver.Dispose();
        if (m_Connections.IsCreated)
            m_Connections.Dispose();
        Debug.Log("Stopped server");
    }
}
