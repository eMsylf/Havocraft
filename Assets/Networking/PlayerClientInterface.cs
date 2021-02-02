using BobJeltes.StandardUtilities;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerClientInterface : MonoBehaviour
{
    [SerializeField]
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
        Debug.LogError("Turn start: " + Player.name);
    }

    internal void TurnEnd()
    {
        Player.PlayerController.enabled = false;
        Debug.LogError("Turn end" + Player.name);
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
        if (isWinner)
            Player.endGameScreen.ScoreText.UpdateValue(100);
    }
}
