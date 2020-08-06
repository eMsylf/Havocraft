using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Canvas))]
public class MenuManager : MonoBehaviour
{
    public Transform Screens;
    public GameObject DebugNavigation;
    public GameObject InputBlocker;
    [Min(0f)]
    public float ScreenTransitionDuration = .5f;

    public int GetScreenCount()
    {
        return transform.childCount;
    }

    public void GoToScreen(Transform screen)
    {
        if (Screens == null)
        {
            Debug.Log("Screens not assigned");
            return;
        }
        if (screen == null)
        {
            Debug.LogError("Screen not assigned");
            return;
        }

        if (ScreenTransitionDuration > 0f)
        {
            InputBlockerActive(true);
            Screens.DOMove(Screens.position + (Screens.position - screen.position), ScreenTransitionDuration).OnComplete(() => InputBlockerActive(false));
        }
        else
        {
            Screens.position = Screens.position + (Screens.position - screen.position);
        }
    }

    public void NextScreen()
    {
        Debug.Log("Next Screen");
        if (Screens == null)
        {
            return;
        }
        if (ScreenTransitionDuration > 0f)
        {
            InputBlockerActive(true);
            Screens.DOMoveX(Screens.position.x -Screen.width, ScreenTransitionDuration).OnComplete(() => InputBlockerActive(false));
        }
        else
            Screens.Translate(-Screen.width, 0f, 0f);
    }
    
    void InputBlockerActive(bool enabled)
    {
        if (InputBlocker != null)
        {
            InputBlocker.SetActive(enabled);
        }
    }

    public void PreviousScreen()
    {
        Debug.Log("Previous Screen");
        if (Screens == null)
        {
            return;
        }
        if (ScreenTransitionDuration > 0f)
        {
            InputBlockerActive(true);
            Screens.DOMoveX(Screens.position.x + Screen.width, ScreenTransitionDuration).OnComplete(() => InputBlockerActive(false));
        }
        else
            Screens.Translate(Screen.width, 0f, 0f);
    }

    public void Open()
    {
        Screens?.gameObject.SetActive(true);
    }

    public void Close()
    {
        Screens?.gameObject.SetActive(false);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
