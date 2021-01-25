using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConditionalButton : MonoBehaviour
{
    public enum Conditions
    {
        PlayerPrefsHas
    }
    public Conditions condition;
    public string playerPrefsKey;
    [Space]
    public string sceneTargetIfTrue;
    public string sceneTargetIfFalse;

    public void Click()
    {
        switch (condition)
        {
            case Conditions.PlayerPrefsHas:
                if (PlayerPrefs.HasKey(playerPrefsKey))
                    SceneManager.LoadScene(sceneTargetIfTrue);
                else
                    SceneManager.LoadScene(sceneTargetIfFalse);
                break;
            default:
                break;
        }
    }
}
