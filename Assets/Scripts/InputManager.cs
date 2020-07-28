using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogWarning("Instance of Input Manager was not set");
                instance = FindObjectOfType<InputManager>();
                if (instance == null)
                {
                    Debug.LogError("No instance of Input Manager found in scene.");
                    instance = new InputManager();
                }
            }
            return instance;
        }
    }

    public InputActionAsset actionmap;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
