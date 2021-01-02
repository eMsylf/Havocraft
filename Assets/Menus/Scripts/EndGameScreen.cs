using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.Menu
{
    public class EndGameScreen : MonoBehaviour
    {
        private bool isWinner;
        public bool IsWinner
        {
            get
            {
                return isWinner;
            }
            set
            {
                Debug.Log("Is Winner set to " + value);
                isWinner = value;
            }
        }
        public GameObject WinnerText;
        public GameObject LoserText;

        private void OnEnable()
        {
            Debug.Log("Is winner: " + IsWinner, this);
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
            WinnerText.SetActive(IsWinner);
            LoserText.SetActive(!IsWinner);
        }
    }
}