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
    public Projectile projectilePrefab;
    public List<Projectile> projectiles = new List<Projectile>();
    public void UpdateProjectileListLength(int newCount)
    {
        while (projectiles.Count < newCount)
        {
            projectiles.Add(Instantiate(projectilePrefab));
        }
        if (projectiles.Count > newCount)
        {
            projectiles.RemoveRange(0, projectiles.Count - newCount);
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
        NativeArray<byte> playerIDBytes = new NativeArray<byte>(BitConverter.GetBytes(playerID), Allocator.Temp);
        NetworkMessage.Send(ClientMessage.PlayerID, this, playerIDBytes);
    }

    #region Send

    public void MovementInputChanged()
    {
        NetworkMessage.Send(ClientMessage.MovementInput, this, default);
    }

    public void ShootingChanged(bool isShooting)
    {
        NativeArray<byte> isShootingBytes = new NativeArray<byte>(BitConverter.GetBytes(isShooting), Allocator.Temp);
        NetworkMessage.Send(ClientMessage.ShootInput, this, isShootingBytes);
    }

    //internal void QuitGame()
    //{
    //    throw new NotImplementedException();
    //}

    #endregion

    #region Receive

    internal void SaveConnectionID(int connectionID)
    {
        clientInfo.connectionID = connectionID;
    }

    internal void TurnEnd()
    {
        PlayerClientInterface pci = PlayerClientInterface.Instance;
        pci.TurnEnd();
    }

    internal void TurnStart()
    {
        PlayerClientInterface pci = PlayerClientInterface.Instance;
        pci.TurnStart();
    }

    internal void ScoreUpdate(int score)
    {
        PlayerClientInterface pci = PlayerClientInterface.Instance;
        pci.UpdateScore(score);
    }

    public string playerScene;
    public string stageScene;
    internal void GameStart(int playerCount)
    {
        DontDestroyOnLoad(gameObject);
        AsyncOperation async = SceneManager.LoadSceneAsync(stageScene);
        async.completed += _ => PlayerReady(async);
        for (int i = 0; i < playerCount-1; i++)
        {
            if (clientInfo.connectionID == i)
            {
                PlayerController controlledPlayer = Instantiate(playerPrefab);
                PlayerClientInterface.Instance.Player = controlledPlayer.GetComponent<Player>();
                players.Add(new NetworkPlayerInfo(clientInfo.id, controlledPlayer, true));
            }
            else
                players.Add(new NetworkPlayerInfo(-1, Instantiate(opponentPrefab), true));
        }
        SetPlayerSpawns();
    }

    public GameManager gameManager;
    public void SetPlayerSpawns()
    {
        for (int i = 0; i < players.Count; i++)
        {
            NetworkPlayerInfo player = players[i];
            player.controller.Rigidbody.position = gameManager.SpawnPoints[i].position;
            player.controller.Rigidbody.rotation = gameManager.SpawnPoints[i].rotation;
        }
    }

    void PlayerReady(AsyncOperation async)
    {
        //SceneManager.LoadScene(stageScene, LoadSceneMode.Additive);
        NetworkMessage.Send(ClientMessage.PlayerReady, this, default);
        async.completed -= _ => PlayerReady(async);
    }

    internal void GameOver(int playerIDWinner)
    {
        PlayerClientInterface pci = PlayerClientInterface.Instance;
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

    internal void UpdatePlayerPositions(List<Vector3> positions)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (i == clientInfo.connectionID)
            {
                PlayerClientInterface.Instance.Player.PlayerController.Rigidbody.MovePosition(positions[i]);
                continue;
            }
            else
            {
                players[i].controller.Rigidbody.MovePosition(positions[i]);
            }
        }
    }

    internal void UpdatePlayerRotations(List<Vector3> rotations)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (i == clientInfo.connectionID)
            {
                PlayerClientInterface.Instance.Player.PlayerController.Rigidbody.MoveRotation(Quaternion.Euler(rotations[i]));
            }
            else
            {
                players[i].controller.Rigidbody.MoveRotation(Quaternion.Euler(rotations[i]));
            }
        }
    }

    internal void UpdateProjectilePositions(List<Vector3> newPositions)
    {
        UpdateProjectileListLength(newPositions.Count);
        for (int i = 0; i < projectiles.Count; i++)
        {
            projectiles[i].Rigidbody.MovePosition(newPositions[i]);
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

    internal void PlayerTakesDamage(NativeArray<byte> playerDamageData)
    {
        byte[] dataBytes = new byte[playerDamageData.Length];
        playerDamageData.CopyTo(dataBytes);
        int receiverPlayerID = BitConverter.ToInt32(dataBytes, 0);
        int dealerPlayerID = BitConverter.ToInt32(dataBytes, 4);
        float damage = BitConverter.ToSingle(dataBytes, 8);

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
