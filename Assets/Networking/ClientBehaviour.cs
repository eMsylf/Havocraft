using System;
using System.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using BobJeltes;
using BobJeltes.StandardUtilities;
using Unity.Collections;

public class ClientBehaviour : Singleton<ClientBehaviour>
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
    private Vector2 movementInput;
    public Vector2 MovementInput
    {
        get => movementInput;
        set
        {
            movementInput = value;
            MovementInputChanged();
        }
    }

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

            if (m_Connection.GetState(m_Driver) == NetworkConnection.State.Connected)
            {
                
                break;
            }
            connectionTime += interval;
            if (connectionTime >= connectionTimeOut)
            {
                Debug.LogError("Connection timed out.", this);
                Disconnect(DisconnectionReason.Timeout);
                break;
            }
        }
    }

    public void SendPlayerID(int playerID)
    {
        NativeArray<byte> playerIDBytes = new NativeArray<byte>();
        playerIDBytes.CopyFrom(BitConverter.GetBytes(playerID));
        NetworkMessage.Send(ClientMessage.PlayerID, this, playerIDBytes);
    }

    PlayerClientInterface playerClientInterface;

    public PlayerClientInterface GetPlayerClientInterface()
    {
        if (playerClientInterface == null)
        {
            playerClientInterface = FindObjectOfType<PlayerClientInterface>();
            if (playerClientInterface == null)
                Debug.LogError("No player-client interface found in scene");
        }
        return playerClientInterface;
    }

    #region Send

    public void MovementInputChanged()
    {
        NetworkMessage.Send(ClientMessage.MovementInput, this, default);
    }

    public void ShootingChanged(bool isShooting)
    {
        NativeArray<byte> isShootingBytes = new NativeArray<byte>();
        
        isShootingBytes.CopyFrom(BitConverter.GetBytes(isShooting));
        
        NetworkMessage.Send(ClientMessage.ShootInput, this, isShootingBytes);
    }

    internal void QuitGame()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Receive

    internal void TurnEnd()
    {
        PlayerClientInterface pci = GetPlayerClientInterface();
        pci.TurnEnd();
    }

    internal void TurnStart()
    {
        PlayerClientInterface pci = GetPlayerClientInterface();
        pci.TurnStart();
    }

    internal void ScoreUpdate(int score)
    {
        PlayerClientInterface pci = GetPlayerClientInterface();
        pci.UpdateScore(score);
    }

    public string playerScene;
    public string stageScene;
    internal void GameStart()
    {
        DontDestroyOnLoad(gameObject);
        AsyncOperation async = SceneManager.LoadSceneAsync(playerScene);
        async.completed += _ => QueueLoadComplete(async);
    }

    void QueueLoadComplete(AsyncOperation async)
    {
        SceneManager.LoadScene(stageScene, LoadSceneMode.Additive);
        NetworkMessage.Send(ClientMessage.PlayerReady, this, default);
        async.completed -= _ => QueueLoadComplete(async);
    }

    internal void GameOver(int playerIDWinner)
    {
        PlayerClientInterface pci = GetPlayerClientInterface();
        clientInfo.UpdateInfo();
        pci.GameOver(playerIDWinner == clientInfo.id);
    }

    public void ReportConnectionState()
    {
        if (m_Connection.IsCreated) Debug.Log("Connection state: " + m_Connection.GetState(m_Driver).ToString());
        else Debug.LogError(name + "has no active connection");
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

    #endregion 

    #region Transport tutorial

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

    #endregion

    [System.Serializable]
    public class ClientInfo
    {
        public int id;
        public string name;

        public void UpdateInfo()
        {
            id = PlayerPrefs.GetInt("player_id");
            name = PlayerPrefs.GetString("username");
        }
    }
    public ClientInfo clientInfo = new ClientInfo();
}
