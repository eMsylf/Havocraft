using BobJeltes.Networking;
using BobJeltes.StandardUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ServerBehaviour : Singleton<ServerBehaviour>
{
    public bool AutoStart = true;
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
    public List<NetworkPlayerInfo> players = new List<NetworkPlayerInfo>();
    public PlayerController playerPrefab;
    public List<Projectile> projectiles = new List<Projectile>();
    public int playersRequiredForGameStart = 2;
    public int playersReady = 0;

    public bool GameIsOngoing = false;

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

            players.Add(new NetworkPlayerInfo(0, Instantiate(playerPrefab).GetComponent<PlayerController>(), true));

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
                    NetworkMessage.Read(this, i, stream);
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

    [Min(0)]
    public float turnDuration = 5f;
    public int currentTurnHolder = 0;

    public IEnumerator ManageTurns()
    {
        while (GameIsOngoing)
        {
            yield return new WaitForSeconds(turnDuration);
            SwitchTurns();
        }
    }

    public void SwitchTurns()
    {
        // Switch turn to other player
        NetworkMessage.Send(ServerMessage.TurnEnd, new NativeArray<byte>(), this, m_Connections[currentTurnHolder]);

        // If currentTurnHolder reaches m_Connections.Length, currentTurnHolder is set back to 0
        currentTurnHolder = ((currentTurnHolder + 1) % m_Connections.Length);
        NetworkMessage.Send(ServerMessage.TurnStart, new NativeArray<byte>(), this, m_Connections[currentTurnHolder]);
    }

    #region Send

    // Initiate scene load for all clients
    public void StartGame()
    {
        StartCoroutine(AnnounceGameStart());
        GameIsOngoing = true;
        StartCoroutine(ManageTurns());
        SceneManager.LoadScene("stage0", LoadSceneMode.Additive);
        SceneManager.LoadScene("servercamera", LoadSceneMode.Additive);
        SetPlayerSpawns();
    }

    public void SetPlayerSpawns()
    {
        for (int i = 0; i < players.Count; i++)
        {
            NetworkPlayerInfo player = players[i];
            player.controller.Rigidbody.position = GameManager.Instance.SpawnPoints[i].position;
            player.controller.Rigidbody.rotation = GameManager.Instance.SpawnPoints[i].rotation;
        }
    }

    public IEnumerator AnnounceGameStart()
    {
        yield return new WaitForSeconds(1f);
        NetworkMessage.SendConnectionIDs(this);
        NetworkMessage.SendAll(ServerMessage.GameStart, new NativeArray<byte>(m_Connections.Length, Allocator.None), this, m_Connections);
    }

    public void SendProjectileImpact(Projectile projectile)
    {
        projectiles.Remove(projectile);
        NetworkMessage.SendAll(ServerMessage.ProjectileImpact, NetworkMessageUtilities.Vector3ToByteNativeArray(projectile.transform.position), this, m_Connections);
    }

    internal void PlayerTakesDamage(Player receiver, float damage, Player dealer)
    {
        // Damage
        NativeArray<byte> damageData = new NativeArray<byte>();
        damageData.CopyFrom(BitConverter.GetBytes(receiver.ID));
        damageData.CopyFrom(BitConverter.GetBytes(dealer.ID));
        damageData.CopyFrom(BitConverter.GetBytes(damage));
        NetworkMessage.SendAll(ServerMessage.PlayerTakesDamage, damageData, this, m_Connections);

        // Score update
        NativeArray<byte> scoreData = new NativeArray<byte>();
        scoreData.CopyFrom(BitConverter.GetBytes(damage));
        NetworkMessage.Send(ServerMessage.ScoreUpdate, scoreData, this, m_Connections[dealer.ID]);
    }

    public void GameOver(int playerIDWinner, int playerIDSecond, float score)
    {
        GameIsOngoing = false;

        // Send game over
        NativeArray<byte> winnerIDBytes = new NativeArray<byte>();
        winnerIDBytes.CopyFrom(BitConverter.GetBytes(playerIDWinner));
        NetworkMessage.SendAll(ServerMessage.GameOver, winnerIDBytes, this, m_Connections);

        // Send game result to database
        StartCoroutine(
            SendScore(
            DatabaseCommunication.GetScoreSendURI((int)score, playerIDWinner, playerIDSecond)
            )
            );
    }

    public IEnumerator SendScore(string uri)
    {
        UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Get(uri);
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.LogError("Error sending send-score request to server");
            yield break;
        }

        string text = webRequest.downloadHandler.text;
        Debug.Log("Score send result: " + text);
    }

    #endregion
    #region Receive

    internal void AssignPlayerIDToConnection(int connectionID, int playerID)
    {
        players[connectionID].playerID = playerID;
    }

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

    internal void ShootInput(int connectionID, bool isShooting)
    {
        players[connectionID].controller.SetShootingActive(isShooting);
    }

    //internal void ClientQuit(int playerID)
    //{
    //    NetworkMessage.SendAll(ServerMessage.Disconnection, DisconnectionReason.)
    //}

    internal void ReadMovementInput(Vector2 input, int connectionID)
    {
        players[connectionID].input = input;
    }

    #endregion

    private void FixedUpdate()
    {
        if (GameIsOngoing)
        {
            UpdatePlayerPositions();
            UpdateProjectilePositions();
        }
    }

    public void UpdatePlayerPositions()
    {
        List<Vector3> rotations = new List<Vector3>();
        foreach (NetworkPlayerInfo player in players)
        {
            player.controller.ApplyForces(player.input);
            rotations.Add(player.controller.Rigidbody.rotation.eulerAngles);
        }
        NetworkMessage.SendAll(ServerMessage.PlayerRotations, NetworkMessageUtilities.Vector3ListToByteNativeArray(rotations), this, m_Connections);
    }

    public void UpdateProjectilePositions()
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (Projectile projectile in projectiles)
        {
            positions.Add(projectile.transform.position);
        }
        NetworkMessage.SendAll(ServerMessage.ProjectilePositions, NetworkMessageUtilities.Vector3ListToByteNativeArray(positions), this, m_Connections);
    }
}
