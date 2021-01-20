using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Login : MonoBehaviour
{
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;

    private void Start()
    {
        UserInfo.ClearSessionData();
        if (PlayerPrefs.HasKey("username"))
        {
            UsernameInput.text = PlayerPrefs.GetString("username");
            UsernameInput.textComponent.text = UsernameInput.text;
            //UsernameInput.SetTextWithoutNotify(PlayerPrefs.GetString("username"));
        }
    }

    public void Submit()
    {
        // Check if all fields are entered
        string username = UsernameInput.text;
        string password = PasswordInput.text;
        int errors = 0;

        if (string.IsNullOrWhiteSpace(username))
        {
            Debug.LogError("Username left blank");
            errors++;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            Debug.LogError("Password left blank.");
            errors++;
        }

        if (errors > 0)
            return;

        string uri = "https://studenthome.hku.nl/~bob.jeltes/client-login.php?" +
            "username=" + username +
            "&password=" + password;

        Debug.Log("URI: " + uri);
        StartCoroutine(ExtractJSON(uri));
    }

    public UserInfo info;

    public IEnumerator ExtractJSON(string uri)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.LogError("Error sending request to server");
            yield break;
        }

        string text = webRequest.downloadHandler.text;
        Debug.Log("Web request result: " + text);
        if (text.Length <= 2)
        {
            Debug.Log("Wrong username or password");
        }

        info = JsonUtility.FromJson<UserInfo>(text);

        info.SaveToPlayerPrefs();
    }
}
