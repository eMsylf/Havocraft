using System;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class EditUserInfo : MonoBehaviour
{
    public TextMeshProUGUI pageTitle;
    [Space]
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;
    public TMP_Dropdown GenderInput;
    public TMP_InputField DateOfBirthInput;

    public UnityEvent OnUsernameBlank;
    public UnityEvent OnPasswordBlank;
    public UnityEvent OnInvalidDate;
    public bool IsNewUser()
    {
        if (PlayerPrefs.HasKey("session_id"))
        {
            return string.IsNullOrEmpty(PlayerPrefs.GetString("session_id"));
        }
        return true;
    }

    void Start()
    {
        if (!IsNewUser())
        {
            pageTitle.text = "Edit user info";
            // Display user info in form
            UsernameInput.text = PlayerPrefs.GetString("username");
            UsernameInput.textComponent.text = UsernameInput.text;
            GenderInput.value = PlayerPrefs.GetInt("gender");
            DateOfBirthInput.text = PlayerPrefs.GetString("date_of_birth");
            DateOfBirthInput.textComponent.text = DateOfBirthInput.text;
        }
        else
            pageTitle.text = "Register new user";
    }

    public void Submit()
    {
        // Check if all fields are entered
        string user_id = PlayerPrefs.HasKey("player_id") ? PlayerPrefs.GetInt("player_id").ToString() :"";
        string username = UsernameInput.text;
        string password = PasswordInput.text;
        int gender = GenderInput.value;
        string dateOfBirth = DateOfBirthInput.text;
        int errors = 0;
        string errorMessage = "";
        if (string.IsNullOrWhiteSpace(username))
        {
            Debug.LogError("Username left blank.");
            OnUsernameBlank.Invoke();
            errors++;
            errorMessage += "Username left blank. \n";
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            Debug.LogError("Password left blank.");
            OnPasswordBlank.Invoke();
            errors++;
            errorMessage += "Password left blank. \n";
        }

        // Check if valid date
        DateTime dateValue;
        if (!System.DateTime.TryParse(dateOfBirth, out dateValue))
        {
            //Invalid date
            Debug.LogError("Invalid date of birth: " + dateOfBirth + ". ");
            OnInvalidDate.Invoke();
            errors++;
            errorMessage += "Invalid date of birth: " + dateOfBirth + " \n";
        }

        if (errors > 0)
        {
            Error.Invoke(errorMessage);
            return;
        }

        string uri = "https://studenthome.hku.nl/~bob.jeltes/hovercraft-havoc/process-user-info?" +
            (!string.IsNullOrWhiteSpace(user_id) ? "id=" + user_id + "&" : "") +
            "username=" + username +
            "&password=" + password +
            "&gender=" + gender.ToString() +
            "&date_of_birth=" + dateOfBirth.ToString();

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
            Error.Invoke("Error sending request to server");
            yield break;
        }

        string text = webRequest.downloadHandler.text;
        Debug.Log("Web request result: " + text);
        if (text.Length <= 2)
        {
            if (IsNewUser())
            {
                Debug.Log("Registration successful");
                Success.Invoke("Registration successful");
            }
            else
            {
                Debug.Log("Info edit successful");
                Success.Invoke("Info edit successful");
            }
            yield break;
        }
        Error.Invoke("Unknown error occurred");
    }

    public UnityEventString Success;
    public UnityEventString Error;
}
