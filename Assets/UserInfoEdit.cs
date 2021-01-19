using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Net;
using UnityEngine.Networking;

public class UserInfoEdit : MonoBehaviour
{
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;
    public TMP_Dropdown GenderInput;
    public TMP_InputField DateOfBirthInput;

    public bool NewUser()
    {
        throw new NotImplementedException();
    }

    void Start()
    {
        if (!NewUser())
        {
            // Display user info in form
        }
    }

    public void Submit()
    {
        // Check if all fields are entered
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

        string uri = "https://studenthome.hku.nl/~bob.jeltes/process-user-info?" +
            "username=" + username +
            "&password=" + password +
            "&gender=" + gender.ToString() +
            "&date_of_birth=" + dateOfBirth.ToString();

        Debug.Log("URI: " + uri);

        UnityWebRequest.Get(uri).SendWebRequest();
    }
}
