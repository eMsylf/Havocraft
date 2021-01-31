using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkPlayerInfo
{
    public int playerID = 0;
    public Vector2 input = new Vector2();
    public PlayerController controller;

    public NetworkPlayerInfo(int _playerID, PlayerController _controller, bool serverControlled)
    {
        playerID = _playerID;
        controller = _controller;
        controller.ServerControlled = serverControlled;
    }
}
