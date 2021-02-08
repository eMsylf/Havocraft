using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BobJeltes.Menu
{
    public class EndGameScreen : MonoBehaviour
    {
        public Graphic Background;
        public GameObject WinnerText;
        public GameObject LoserText;
        public ValueText ScoreText;

        public void ShowWinner(float score = 0)
        {
            Activate(true, score);
        }

        public void ShowLoser(float score = 0)
        {
            Activate(false, score);
        }

        public void Activate(float score)
        {
            Debug.Log("Update score text to " + score, this);
            if (ScoreText != null) ScoreText.UpdateValue(score);
        }

        public void Activate(bool winner, float score = 0)
        {
            if (Background != null) Background.enabled = true;
            Debug.Log("Is winner: " + winner, this);
            if (WinnerText == null)
            {
                Debug.LogError("Winner text missing", this);
                return;
            }
            if (LoserText == null)
            {
                Debug.LogError("Loser text missing", this);
                return;
            }
            WinnerText.SetActive(winner);
            LoserText.SetActive(!winner);
            if (ScoreText != null) ScoreText.UpdateValue(score);
        }

    }
}