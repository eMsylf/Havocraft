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

    public Transform StartScreen;
    [Tooltip("Assigns the startscreen by which screen comes first in the children hierarchy")]
    public bool AssignAutomatically = true;
    public Transform CurrentScreen = null;
    public Transform PreviousScreen = null;

    public int GetScreenCount()
    {
        return transform.childCount;
    }

    public void GoToScreen(Transform newScreen)
    {
        if (Screens == null)
        {
            Debug.Log("Screens not assigned");
            return;
        }
        if (newScreen == null)
        {
            Debug.LogError("Screen not assigned");
            return;
        }

        if (ScreenTransitionDuration > 0f)
        {
            InputBlockerActive(true);
            Screens.DOMove(GetMoveDelta(CurrentScreen, newScreen), ScreenTransitionDuration).OnComplete(() => InputBlockerActive(false));
        }
        else
        {
            Screens.position = GetMoveDelta(CurrentScreen, newScreen);
        }
        UpdateScreens(newScreen, CurrentScreen);
    }

    public Vector3 GetMoveDelta(Transform current, Transform next)
    {
        return Screens.position + (current.position - next.position);
    }

    public void UpdateScreens(Transform current, Transform previous)
    {
        CurrentScreen = current;
        PreviousScreen = previous;
    }

    public void GoToNextScren()
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

    public void GoToPreviousScreen()
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
        if (AssignAutomatically)
        {
            StartScreen = Screens.childCount != 0 ? Screens.GetChild(0):null;
        }
        CurrentScreen = StartScreen;
    }

    void Update()
    {
        
    }
}
