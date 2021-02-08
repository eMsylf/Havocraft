using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.StandardUtilities;

public class ScoreManager : Singleton<ScoreManager>
{
    public int Score = 0;
    public ValueText ScoreDisplay;

    public void SetScore(int score)
    {
        Score = score;
        if (ScoreDisplay != null)
        {
            ScoreDisplay.UpdateValue(score);
        }
    }

    public void AddScore(int addition)
    {
        SetScore(Score + addition);
    }
}
