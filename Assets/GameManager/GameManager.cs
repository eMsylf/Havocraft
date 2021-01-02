using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using BobJeltes.Menu;
using UnityEngine;
using BobJeltes.StandardUtilities;

public class GameManager : Singleton<GameManager>
{
    protected GameManager() { }

    public EndGameScreen EndGameScreenPrefab;
    private EndGameScreen EndGameScreenInstance;

    public List<PlayerController> Players = new List<PlayerController>();

    private void OnEnable()
    {
        CollectPlayers();
    }

    public void CollectPlayers()
    {
        Players = FindObjectsOfType<PlayerController>().ToList();
        PrintPlayers();
    }

    public string PrintPlayers()
    {
        string players = "";
        foreach (PlayerController player in Players)
        {
            players += "\n" + player.name;
        }
        Debug.Log(Players.Count + " players: " + players, this);
        return players;
    }

    public void SceneLoad(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void SceneReload()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void SceneLoadNext()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Debug.Log("Quit game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PlayerDeath(PlayerController player)
    {
        Players.Remove(player);
        PrintPlayers();
        if (Players.Count == 1)
        {
            MatchComplete();
        }
    }

    public void MatchComplete()
    {
        Debug.Log("Match complete");
        if (EndGameScreenInstance == null)
        {
            EndGameScreenInstance = Instantiate(EndGameScreenPrefab);
            EndGameScreenInstance.gameObject.SetActive(false);
        }
        EndGameScreenInstance.IsWinner = true;
        EndGameScreenInstance.gameObject.SetActive(true);
    }
}
