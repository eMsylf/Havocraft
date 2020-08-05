using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MenuManager : MonoBehaviour
{
    public Transform Screens;

    public int GetScreenCount()
    {
        return transform.childCount;
    }

    public void NextScreen()
    {
        Debug.Log("Next Screen");
        if (Screens == null)
        {
            return;
        }

        Screens.Translate(-Screen.width, 0f, 0f);
    }

    public void PreviousScreen()
    {
        Debug.Log("Previous Screen");
        if (Screens == null)
        {
            return;
        }
        Screens.Translate(Screen.width, 0f, 0f);

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
