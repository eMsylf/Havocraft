using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sends a score value to the Score Manager upon being disabled
/// </summary>
public class PointValue : MonoBehaviour
{
    public int value = 10;
    private void OnDisable()
    {
        if (ScoreManager.HasActiveInstance())
            ScoreManager.Instance.AddScore(value);
    }
}
