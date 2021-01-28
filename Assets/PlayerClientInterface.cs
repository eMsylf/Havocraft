using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerClientInterface : MonoBehaviour
{
    private Player player;
    public Player Player
    {
        get
        {
            if (player == null)
            {
                player = GetComponent<Player>();
                if (player == null)
                    Debug.LogError("No player component assigned to player client interface", this);
            }
            return player;
        }
        set => player = value;
    }

    [SerializeField]
    private ClientBehaviour clientBehaviour;
    public ClientBehaviour GetClientBehaviour()
    {
        if (clientBehaviour == null)
        {
            clientBehaviour = FindObjectOfType<ClientBehaviour>();
            if (clientBehaviour == null)
            {
                Debug.LogError("No client behaviour fount in scene", this);
            }
        }
        return clientBehaviour;
    }

    // Send
    public void MovementChanged(Vector2 input)
    {
        ClientBehaviour clientBehaviour = GetClientBehaviour();
        if (clientBehaviour == null)
            return;

        clientBehaviour.MovementInput = input;
    }

    public void ShootingChanged(bool val)
    {
        ClientBehaviour clientBehaviour = GetClientBehaviour();
        if (clientBehaviour == null)
            return;

        clientBehaviour.ShootingChanged(val);
    }

    public void QuitGame()
    {
        ClientBehaviour clientBehaviour = GetClientBehaviour();
        if (clientBehaviour == null)
            return;

        clientBehaviour.QuitGame();
    }

    // Read
    internal void TurnStart()
    {
        Player.PlayerController.enabled = true;
    }

    internal void TurnEnd()
    {
        Player.PlayerController.enabled = false;
    }

    internal void UpdateScore(int score)
    {
        Player.ScoreValue.UpdateValue(score);
        Player.endGameScreen.ScoreText.UpdateValue(score);
    }

    internal void GameOver(bool isWinner)
    {
        Player.endGameScreen.gameObject.SetActive(true);
        Player.endGameScreen.Activate(isWinner);
    }
}
