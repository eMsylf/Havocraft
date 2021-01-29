using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine.Events;
using BobJeltes;

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
    public NativeList<NetworkConnection> m_Connections;

    public int playersRequiredForGameStart = 2;
    public int playersReady = 0;

    public UnityEventString OnConnectionCountChanged;

    public UnityEvent OnServerStart;
    public UnityEvent OnServerStop;

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
            if (m_Connections.Length == playersRequiredForGameStart)
            {
                StartGame();
            }
        }

        Ping();

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
                    NetworkMessage.Read(stream, i, this);
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
        Debug.Log("Pinging " + m_Connections.Length + " clients");
        for (int i = 0; i < m_Connections.Length; i++)
        {
            NetworkConnection connection = m_Connections[i];
            if (!connection.IsCreated)
                continue;
            if (connection.GetState(m_Driver) != NetworkConnection.State.Connected)
                continue;
            NetworkMessage.Send(ServerMessage.Ping, new NativeArray<byte>(), this, connection);
        }
        // Reset time
        timeSinceLastPing = 0f;
    }

    internal void ReadPong(int playerID)
    {
        Debug.Log("Received pong from " + playerID);
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
        if (!m_Connections.IsCreated)
            return;

        NativeArray<byte> data = new NativeArray<byte>();
        byte[] bytes = { (byte)DisconnectionReason.ServerStopped };
        data.CopyFrom(bytes);
        NetworkMessage.SendAll(ServerMessage.Disconnection, data, this, m_Connections);
        for (int i = 0; i < m_Connections.Length; i++)
        {
            m_Connections[i].Disconnect(m_Driver);
        }
        if (m_Driver.IsCreated)
            m_Driver.Dispose();
        if (m_Connections.IsCreated)
            m_Connections.Dispose();

        if (ClearIPOnStop) IPAddress = "0.0.0.0";
        Debug.Log("Stopped server");
        OnServerStop.Invoke();
    }

    #region Send

    // Initiate scene load for all clients
    public void StartGame()
    {
        StartCoroutine(AnnounceGameStart());
    }

    public IEnumerator AnnounceGameStart()
    {
        yield return new WaitForSeconds(1f);
        NetworkMessage.SendAll(ServerMessage.GameStart, new NativeArray<byte>(), this, m_Connections);
    }

    internal void PlayerTakesDamage(Player receiver, float damage, Player dealer)
    {
        // Damage
        NativeArray<byte> damageData = new NativeArray<byte>();
        damageData.CopyFrom(BitConverter.GetBytes(receiver.ID));
        damageData.CopyFrom(BitConverter.GetBytes(damage));
        damageData.CopyFrom(BitConverter.GetBytes(dealer.ID));
        NetworkMessage.SendAll(ServerMessage.PlayerTakesDamage, damageData, this, m_Connections);

        // Score update
        NativeArray<byte> scoreData = new NativeArray<byte>();
        scoreData.CopyFrom(BitConverter.GetBytes(damage));
        NetworkMessage.Send(ServerMessage.ScoreUpdate, scoreData, this, m_Connections[dealer.ID]);
    }

    public void GameOver(int playerIDWinner, int playerIDLoser, float score)
    {

    }

    #endregion
    #region Receive

    // Called when a scene load is done
    internal void PlayerReady(int playerID)
    {
        Debug.Log("Player " + playerID + " ready");
        playersReady++;
        if (playersReady >= m_Connections.Length)
        {
            Debug.Log("All players ready");
        }
        else
        {
            Debug.Log(playersReady + " of " + m_Connections.Length + " ready ", this);
        }
    }

    internal void ShootInput(int playerID)
    {
        throw new NotImplementedException();
    }

    internal void ClientQuit(int playerID)
    {
        throw new NotImplementedException();
    }

    internal void ReadMovementInput(float x, float y, int playerID)
    {
        throw new NotImplementedException();
    }

    #endregion

    
}
