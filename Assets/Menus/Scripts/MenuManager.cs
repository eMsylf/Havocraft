using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using BobJeltes;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class MenuManager : MonoBehaviour
{
    public Transform Screens;
    public GameObject DebugNavigation;
    public GameObject InputBlocker;
    [Min(0f)]
    public float ScreenTransitionDuration = .5f;

    public MenuScreen StartScreen;
    [Tooltip("Assigns the startscreen by which screen comes first in the children hierarchy")]
    public bool AssignAutomatically = true;
    public MenuScreen CurrentScreen = null;
    public List<MenuScreen> PreviousScreens = new List<MenuScreen>();

    public int GetScreenCount()
    {
        return transform.childCount;
    }

    public void GoToScreen(MenuScreen newScreen)
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
            Screens.DOMove(GetMoveDelta(CurrentScreen.transform, newScreen.transform), ScreenTransitionDuration).OnComplete(() => InputBlockerActive(false));
        }
        else
        {
            Screens.position = GetMoveDelta(CurrentScreen.transform, newScreen.transform);
        }

        PreviousScreens.Add(CurrentScreen);
        CurrentScreen = newScreen;
    }

    public void GoToPreviousScreen()
    {
        if (Screens == null)
        {
            Debug.Log("Screens not assigned");
            return;
        }
        if (PreviousScreens == null || PreviousScreens.Count == 0)
        {
            Debug.Log("No previous screens to go back to");
            return;
        }
        MenuScreen previousScreen = PreviousScreens[PreviousScreens.Count - 1];
        if (previousScreen == null)
        {
            Debug.Log("Previous screen is null");
            return;
        }

        if (ScreenTransitionDuration > 0f)
        {
            InputBlockerActive(true);
            Screens.DOMove(GetMoveDelta(CurrentScreen.transform, previousScreen.transform), ScreenTransitionDuration).OnComplete(() => InputBlockerActive(false));
        }
        else
        {
            Screens.position = GetMoveDelta(CurrentScreen.transform, previousScreen.transform);
        }

        CurrentScreen = previousScreen;
        PreviousScreens.Remove(previousScreen);
    }

    public Vector3 GetMoveDelta(Transform current, Transform next)
    {
        return Screens.position + (current.position - next.position);
    }

    public void AddScreenToHistory(MenuScreen screen)
    {
        PreviousScreens.Add(screen);
    }

    void InputBlockerActive(bool enabled)
    {
        if (InputBlocker != null)
        {
            InputBlocker.SetActive(enabled);
        }
    }

    public bool IsOpen { get { return GetComponent<Canvas>().enabled; } }

    public void Toggle()
    {
        GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
    }

    public void Open()
    {
        GetComponent<Canvas>().enabled = true;
    }

    public void Close()
    {
        GetComponent<Canvas>().enabled = false;
    }

    void Start()
    {
        if (AssignAutomatically)
        {
            if (Screens.childCount != 0)
            {
                MenuScreen firstScreen = Screens.GetChild(0).GetComponent<MenuScreen>();
                if (firstScreen != null)
                    StartScreen = firstScreen;
            }
        }
        CurrentScreen = StartScreen;
    }

    public bool VisualizeScreenNavigation = true;

    private void OnDrawGizmosSelected()
    {
        if (Screens != null)
        {
            return;
        }

        foreach (MenuButton button in Screens.GetComponentsInChildren<MenuButton>())
        {
            if (button.Target != null) 
                Gizmos.DrawLine(button.transform.position, button.Target.transform.position);
        }

        //foreach (MenuScreen screen in Screens.GetComponentsInChildren<MenuScreen>())
        //{
            
        //}
    }
}
