using BobJeltes.StandardUtilities;
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

    // Send (to client)
    public void MovementChanged(Vector2 input)
    {
        ClientBehaviour clientBehaviour = ClientBehaviour.Instance;
        if (clientBehaviour == null)
            return;

        clientBehaviour.MovementInput = input;
    }

    public void ShootingChanged(bool val)
    {
        ClientBehaviour clientBehaviour = ClientBehaviour.Instance;
        if (clientBehaviour == null)
            return;

        clientBehaviour.ShootingChanged(val);
    }

    public void QuitGame()
    {
        ClientBehaviour clientBehaviour = ClientBehaviour.Instance;
        if (clientBehaviour == null)
            return;

        //clientBehaviour.QuitGame();
    }

    // Read (to player)
    internal void TurnStart()
    {
        Player.PlayerController.enabled = true;
        Debug.LogError("Turn start");
    }

    internal void TurnEnd()
    {
        Player.PlayerController.enabled = false;
        Debug.LogError("Turn end");
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
