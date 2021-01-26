using System.Collections;
using UnityEngine;
using Unity.Networking.Transport;
using BobJeltes;
using System;
using UnityEngine.SceneManagement;

public class ClientBehaviour : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public bool AutoConnect = true;
    public float connectionReportInterval = 1f;
    public float connectionTimeOut = 10f;
    public string IPAddress = "";
    public ushort port = 9000;
    //public bool Done;
    [HideInInspector]
    public uint value = 1;

    [Min(0)]
    public float pingInterval = 1f;
    private float timeSinceLastPing = 1f;
    internal Vector2 movementInput;

    private void Start()
    {
        if (AutoConnect)
            ConnectToServer(false);
    }

    private void OnDestroy()
    {
        if (m_Connection.IsCreated)
            Disconnect(DisconnectionReason.ClientDestroyed);
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

        //PingServer();
        DataStreamReader stream;
        NetworkEvent.Type cmd = m_Connection.PopEvent(m_Driver, out stream); 
        
        //Debug.Log("Network Event Type: " + cmd.ToString());
        switch (cmd)
        {
            case NetworkEvent.Type.Empty:
                break;
            case NetworkEvent.Type.Data:
                Debug.Log(name + " received data");
                NetworkMessage.Read(stream, this);
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
        if (forceReconnection && m_Connection.IsCreated) Disconnect(DisconnectionReason.Reconnection);
        if (!m_Driver.IsCreated) m_Driver = NetworkDriver.Create();
        m_Connection = default;

        //var endpoint = NetworkEndPoint.LoopbackIpv4;
        //endpoint.Port = port;
        m_Connection = m_Driver.Connect(NetworkEndPoint.Parse(IPAddress, port));
        Debug.Log("Attempt connection to " + IPAddress);
        ReportConnectionState();
        StartCoroutine(ConnectStatus(connectionReportInterval));
    }

    public IEnumerator ConnectStatus(float interval)
    {
        if (!m_Driver.IsCreated) yield break;
        if (!m_Connection.IsCreated) yield break;

        float connectionTime = 0f;

        while (m_Driver.IsCreated && m_Connection.IsCreated && m_Connection.GetState(m_Driver) == NetworkConnection.State.Connecting)
        {
            Debug.Log("Connecting...");
            yield return new WaitForSeconds(interval);
            connectionTime += interval;
            if (connectionTime >= connectionTimeOut)
            {
                Debug.LogError("Connection timed out.", this);
                Disconnect(DisconnectionReason.Timeout);
                break;
            }
        }
    }

    internal void TurnEnd()
    {
        throw new NotImplementedException();
    }

    internal void TurnStart()
    {
        throw new NotImplementedException();
    }

    internal void ScoreUpdate(int v)
    {
        throw new NotImplementedException();
    }

    internal void GameOver(byte isWinner)
    {
        throw new NotImplementedException();
    }

    public string gameScene;
    internal void GameStart()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadSceneAsync(gameScene).completed += _ => QueueLoadComplete();
    }

    void QueueLoadComplete()
    {
        NetworkMessage.Send(ClientMessage.SceneLoaded, this);
        SceneManager.LoadSceneAsync(gameScene).completed -= _ => QueueLoadComplete();
    }

    public void ReportConnectionState()
    {
        if (m_Connection.IsCreated) Debug.Log("Connection state: " + m_Connection.GetState(m_Driver).ToString());
        else Debug.LogError(name + "has no active connection");
    }

    
    public void PingServer()
    {
        if (!m_Connection.IsCreated)
            return;
        if (m_Connection.GetState(m_Driver) != NetworkConnection.State.Connected)
            return;

        if (timeSinceLastPing < pingInterval)
        {
            timeSinceLastPing += Time.deltaTime;
            return;
        }
        Debug.Log("Ping");
        var writer = m_Driver.BeginSend(m_Connection);
        m_Driver.EndSend(writer);
        timeSinceLastPing = 0f;
    }
    public void SendValueToServer()
    {
        var writer = m_Driver.BeginSend(m_Connection);
        writer.WriteUInt(value);
        m_Driver.EndSend(writer);
    }

    public void Disconnect(DisconnectionReason reason)
    {
        if (!m_Connection.IsCreated)
        {
            Debug.LogError("No active connection");
            return;
        }
        m_Connection.Disconnect(m_Driver);
        m_Connection = default;
        m_Driver.Dispose();
        Debug.Log(name + " disconnected from the server. Reason: " + reason.ToString(), this);
    }
}
