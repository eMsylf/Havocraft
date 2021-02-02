using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class UserInfo
{
    public string session_id;
    public int player_id;
    public string username;
    public string date_of_birth;
    public int gender;

    public void SaveToPlayerPrefs()
    {
        ClientBehaviour.player_id = player_id;
        PlayerPrefs.SetString("session_id", session_id);
        PlayerPrefs.SetInt("player_id", player_id);
        PlayerPrefs.SetString("username", username);
        PlayerPrefs.SetString("date_of_birth", date_of_birth);
        PlayerPrefs.SetInt("gender", gender);
    }

    public static void ClearSessionData()
    {
        PlayerPrefs.DeleteKey("session_id");
    }

    public static void Logout()
    {
        PlayerPrefs.DeleteKey("session_id");
        PlayerPrefs.DeleteKey("player_id");
    }
}
