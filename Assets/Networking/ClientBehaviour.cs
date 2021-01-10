using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class ClientBehaviour : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public bool Done;
    public uint value = 1;

    void Start()
    {
        m_Driver = NetworkDriver.Create();
        m_Connection = default;

        var endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = 9000;
        m_Connection = m_Driver.Connect(endpoint);
    }

    private void OnDestroy()
    {
        m_Driver.Dispose();
    }

    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            if (!Done)
            {
                Debug.Log("Something went wrong during connection", this);
            }
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {

            Debug.Log("Network Event Type: " + cmd.ToString());
            switch (cmd)
            {
                case NetworkEvent.Type.Empty:
                    break;
                case NetworkEvent.Type.Data:
                    value = stream.ReadUInt();
                    Debug.Log(name + " got the value = " + value + " back from the server");
                    Done = true;
                    m_Connection.Disconnect(m_Driver);
                    m_Connection = default;
                    break;
                case NetworkEvent.Type.Connect:
                    Debug.Log(name + " is now connected to the server", this);

                    var writer = m_Driver.BeginSend(m_Connection);
                    writer.WriteUInt(value);
                    m_Driver.EndSend(writer);
                    break;
                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Client got disconnected from server", this);
                    m_Connection = default;
                    break;
                default:
                    break;
            }
        }
    }
}
