using System;
using System.Collections;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using BobJeltes.StandardUtilities;
using BobJeltes.Networking;
using System.Collections.Generic;

public class ClientBehaviour : Singleton<ClientBehaviour>
{
    public static int player_id = -1;

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

    [System.Serializable]
    public class ClientInfo
    {
        public int connectionID;
        public int id;
        public string name;

        public void UpdateInfo()
        {
            id = PlayerPrefs.GetInt("player_id");
            name = PlayerPrefs.GetString("username");
        }
    }
    public ClientInfo clientInfo = new ClientInfo();
    public List<NetworkPlayerInfo> players;
    public PlayerController playerPrefab;
    public PlayerController opponentPrefab;
    public List<Transform> spawnPoints = new List<Transform>();

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

    public GameObject projectileImpactPrefab;
    public Rigidbody projectilePrefab;
    public List<Rigidbody> projectiles = new List<Rigidbody>();
    public void UpdateProjectileListLength(int newCount)
    {
        while (projectiles.Count < newCount)
        {
            projectiles.Add(Instantiate(projectilePrefab));
        }
        while (projectiles.Count > newCount)
        {
            projectiles.RemoveAt(0);
        }
    }

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
        clientInfo.UpdateInfo();

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
                Debug.Log("Stream length remaining: " + stream.Length);
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

    private void FixedUpdate()
    {
        ApplyPlayerPositionsAndRotations();
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
                SendPlayerID();
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

    public void SendPlayerID()
    {
        NetworkMessage.Send(ClientMessage.PlayerID, this);
    }

    #region Send

    public void MovementInputChanged()
    {
        NetworkMessage.Send(ClientMessage.MovementInput, this);
    }
    public bool IsShooting = false;
    public void ShootingChanged(bool isShooting)
    {
        IsShooting = isShooting;
        Debug.LogError("Shooting changed to " + isShooting);
        NetworkMessage.Send(ClientMessage.ShootInput, this);
    }

    //internal void QuitGame()
    //{
    //    throw new NotImplementedException();
    //}

    #endregion

    #region Receive

    internal void SaveConnectionID(int connectionID)
    {
        Debug.Log("Saved connection ID: " + connectionID);
        clientInfo.connectionID = connectionID;
    }

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
        pci.UpdateScore(score);// of ik moet hier de score bijhouden, of op de server. Op de server lijkt mij logischer
    }

    //public string playerScene;
    public string stageScene;
    internal void GameStart(int playerCount)
    {
        DontDestroyOnLoad(gameObject);
        AsyncOperation async = SceneManager.LoadSceneAsync(stageScene);
        async.completed += _ => PlayerReady(async, playerCount);
    }

    public GameManager gmInstance;
    public GameManager gmPrefab;
    public void SetPlayerSpawns()
    {
        if (gmInstance == null)
            gmInstance = Instantiate(gmPrefab);
        for (int i = 0; i < players.Count; i++)
        {
            NetworkPlayerInfo player = players[i];
            if (player == null) Debug.LogError("Player is null");
            if (player.controller == null) Debug.LogError("Controller of player is null");
            if (player.controller.Rigidbody == null) Debug.LogError("Player is null");
            player.controller.Rigidbody.position = gmInstance.SpawnPoints[i].position;
            player.controller.Rigidbody.rotation = gmInstance.SpawnPoints[i].rotation;
        }
    }

    void PlayerReady(AsyncOperation async, int playerCount)
    {
        //SceneManager.LoadScene(stageScene, LoadSceneMode.Additive);
        for (int i = 0; i < playerCount; i++)
        {
            if (clientInfo.connectionID == i)
            {
                PlayerController controlledPlayer = Instantiate(playerPrefab);
                //GetPlayerClientInterface().Player = controlledPlayer.GetComponent<Player>();
                players.Add(new NetworkPlayerInfo(clientInfo.id, controlledPlayer, true));
                Debug.Log("Spawned the player prefab");
            }
            else
            {
                players.Add(new NetworkPlayerInfo(-1, Instantiate(opponentPrefab), true));
                Debug.Log("Spawned an opponent prefab");
            }
        }
        Debug.Log("Spawned " + playerCount + " players");
        SetPlayerSpawns();
        NetworkMessage.Send(ClientMessage.PlayerReady, this);
        async.completed -= _ => PlayerReady(async, playerCount);
    }

    internal void GameOver(ref DataStreamReader stream)
    {
        PlayerClientInterface pci = GetPlayerClientInterface();
        clientInfo.UpdateInfo();
        int winnerID = stream.ReadInt();
        int secondID = stream.ReadInt();
        int winnerScore = stream.ReadInt();
        pci.GameOver(winnerID == player_id);
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

    public List<Vector3> playerPositions = new List<Vector3>();
    internal void UpdatePlayerPositions(List<Vector3> positions)
    {
        playerPositions = positions;
    }

    public List<Vector3> playerRotations = new List<Vector3>();
    internal void UpdatePlayerRotations(List<Vector3> rotations)
    {
        playerRotations = rotations;
    }

    [Range(0f, 1f)] 
    public float positionSmoothing = .5f;
    public float rotationSmoothing = .5f;
    internal void ApplyPlayerPositionsAndRotations()
    {
        for (int i = 0; i < players.Count; i++)
        {
            Rigidbody rb = players[i].controller.Rigidbody;
            if (playerPositions.Count == players.Count)
            {
                Vector3 smoothPosition = Vector3.Lerp(rb.position, playerPositions[i], positionSmoothing);
                rb.MovePosition(smoothPosition);
            }
            if (playerRotations.Count == players.Count)
            {
                Quaternion smoothRotation = Quaternion.Lerp(rb.rotation, Quaternion.Euler(playerRotations[i]), rotationSmoothing);
                rb.MoveRotation(smoothRotation);
            }
        }
    }

    internal void UpdateProjectilePositions(List<Vector3> newPositions)
    {
        Debug.LogError("Number of projectiles: " + newPositions.Count);
        UpdateProjectileListLength(newPositions.Count);
        for (int i = 0; i < projectiles.Count; i++)
        {
            projectiles[i].MovePosition(newPositions[i]);
        }
    }

    internal void SpawnProjectileImpacts(List<Vector3> impactPositions)
    {
        if (projectileImpactPrefab == null)
        {
            Debug.LogWarning("No projectile impact prefab assigned");
            return;
        }

        foreach (Vector3 position in impactPositions)
        {
            Instantiate(projectileImpactPrefab, position, Quaternion.identity);
        }
    }

    internal void PlayerTakesDamage(ref DataStreamReader stream)
    {
        int receiverPlayerID = stream.ReadInt();
        int dealerPlayerID = stream.ReadInt();
        int damage = stream.ReadInt();

        Player receivingPlayer = null;
        Player dealingPlayer = null;
        foreach (NetworkPlayerInfo playerInfo in players)
        {
            if (playerInfo.playerID == receiverPlayerID)
            {
                receivingPlayer = playerInfo.controller.GetComponent<Player>();
            }
            if (playerInfo.playerID == dealerPlayerID)
            {
                dealingPlayer = playerInfo.controller.GetComponent<Player>();
            }
        }
        if (receivingPlayer != null)
            GameManager.Instance.PlayerTakesDamage(receivingPlayer, damage, dealingPlayer);
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
}
