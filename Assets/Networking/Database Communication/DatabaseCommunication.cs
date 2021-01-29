using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DatabaseCommunication
{
    public static readonly string uriBase = "https://studenthome.hku.nl/~bob.jeltes/hovercraft-havoc/";
    public static readonly string clientLoginSubdomain = "client-login.php";
    public static readonly string serverLoginSubdomain = "server-login.php?id=1&password=admin";
    public static readonly string scoreSubdomain = "send-score.php";

    public static string GetScoreSendURI(int score, int playerIDWinner, int playerIDSecond)
    {
        return
            uriBase +
            scoreSubdomain +
            "?" +
            "score=" + score +
            "&" +
            "player_id_winner=" + playerIDWinner +
            "&" +
            "player_id_second=" + playerIDSecond;

    }

    public static string GetClientLoginURI(string username, string password)
    {
        return
            uriBase +
            clientLoginSubdomain +
            "?" + 
            "username=" + username +
            "&" +
            "password=" + password;

    }

    public static string GetServerLoginURI(int serverID, string serverPassword)
    {
        return
           "server-login.php?" +
           "id=" + serverID +
           "&" +
           "password=" + serverPassword;
    }
}
