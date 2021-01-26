using UnityEngine;

public class OpenDevConsoleScript : MonoBehaviour
{
    void Start()
    {
        Debug.developerConsoleVisible = true;
        Debug.LogError("Open die dev console dan jonge");
    }
}