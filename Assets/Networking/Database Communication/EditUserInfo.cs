using System;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class EditUserInfo : MonoBehaviour
{
    public TextMeshProUGUI pageTitle;
    [Space]
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;
    public TMP_Dropdown GenderInput;
    public TMP_InputField DateOfBirthInput;
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

        // Check if valid date
        DateTime dateValue;
        if (!System.DateTime.TryParse(dateOfBirth, out dateValue))
        {
            //Invalid date
            Debug.LogError("Invalid date of birth: " + dateOfBirth);
            errors++;
        }

        if (errors > 0)
            return;

        string uri = "https://studenthome.hku.nl/~bob.jeltes/hovercraft-havoc/process-user-info?" +
            (!string.IsNullOrWhiteSpace(user_id) ? "id=" + user_id + "&" : "") +
            "username=" + username +
            "&password=" + password +
            "&gender=" + gender.ToString() +
            "&date_of_birth=" + dateOfBirth.ToString();

        Debug.Log("URI: " + uri);

        UnityWebRequest.Get(uri).SendWebRequest();
    }
}
