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

    private void Start()
    {
        Players = FindObjectsOfType<PlayerController>().ToList();
    }

    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void NextScene()
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
        if (Players.Count == 1)
        {
            MatchComplete();
        }
    }

    public void MatchComplete()
    {
        if (EndGameScreenInstance == null)
        {
            EndGameScreenInstance = Instantiate(EndGameScreenPrefab);
            EndGameScreenInstance.gameObject.SetActive(false);
        }
        EndGameScreenInstance.IsWinner = true;
        EndGameScreenInstance.gameObject.SetActive(true);
    }
}
