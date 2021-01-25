using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UsernameDisplayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("username");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
