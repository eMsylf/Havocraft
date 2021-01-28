using System.Collections.Generic;
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

    public void PlayerDeath(Player player)
    {
        Players.Remove(player);
        PrintPlayers();
        if (Players.Count <= 1)
        {
            MatchComplete();
        }
    }

    public void MatchComplete()
    {
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
