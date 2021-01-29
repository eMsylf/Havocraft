﻿using System.Collections.Generic;
using System.Linq;
using BobJeltes.Menu;
using BobJeltes.StandardUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    protected GameManager() { }

    public EndGameScreen EndGameScreenPrefab;
    private EndGameScreen EndGameScreenInstance;

    public List<Player> Players = new List<Player>();

    public ServerBehaviour serverBehaviour;
    public ServerBehaviour Server
    {
        get
        {
            if (serverBehaviour == null)
            {
                serverBehaviour = FindObjectOfType<ServerBehaviour>();
                if (serverBehaviour == null)
                    Debug.LogError("No server behaviour found in scene");
            }
            return serverBehaviour;
        }
    }

    private void OnEnable()
    {
        CollectPlayers();
    }

    private void OnSceneLoad()
    {
        CollectPlayers();
    }

    public void CollectPlayers()
    {
        Players = FindObjectsOfType<Player>().ToList();
        PrintPlayers();
    }

    public string PrintPlayers()
    {
        string playerNames = "";
        foreach (Player player in Players)
        {
            if (player != null)
                playerNames += "\n" + player.name;
        }
        Debug.Log(Players.Count + " players: " + playerNames, this);
        return playerNames;
    }

    public void SceneLoad(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        OnSceneLoad();
    }

    public void SceneReload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        OnSceneLoad();
    }

    public void SceneLoadNext()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        OnSceneLoad();
    }

    public void Quit()
    {
        Debug.Log("Quit game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
#else
        Application.Quit();
#endif
    }

    public void PlayerTakesDamage(Player receiver, float damage, Player dealer = null)
    {
        if (Server != null)
        {
            // Report damage to server
            Server.PlayerTakesDamage(receiver, damage, dealer);
        }
        else
        {
            // Deal damage directly
            receiver.TakeDamage(damage);
        } 
    }

    public void PlayerDeath(Player player)
    {
        Players.Remove(player);
        PrintPlayers();
        if (Players.Count == 1)
        {
            MatchComplete(Players[0], player);
        }
    }

    public void MatchComplete(Player winner, Player second)
    {
        if (Server != null)
        {
            Server.GameOver(winner.ID, second.ID, winner.ScoreValue.Value);
            return;
        }

        Debug.Log(Players.Count + " players remaining. Match complete");
        if (EndGameScreenInstance == null)
        {
            if (EndGameScreenPrefab != null)
                EndGameScreenInstance = Instantiate(EndGameScreenPrefab);
            else
            {
                EndGameScreenInstance = FindObjectOfType<EndGameScreen>();
                if (EndGameScreenInstance == null)
                {
                    Debug.LogError("No End Game Screen instance found.");
                    return;
                }
            }
        }

        EndGameScreenInstance.gameObject.SetActive(true);
        EndGameScreenInstance.Activate(true);
    }
}
