using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BobJeltes.Menu
{
    [RequireComponent(typeof(Button))]
    public class NavigationButton : MonoBehaviour
    {
        private MenuManager MenuManager;
        public Transform TargetMenuScreen;
        public bool IsBackButton;

        private MenuManager GetMenuManager()
        {
            if (MenuManager == null)
            {
                MenuManager = GetComponentInParent<MenuManager>();
                if (MenuManager == null)
                {
                    Debug.LogError("Menu Manager is missing", gameObject);
                }
            }
            return MenuManager;
        }

        public void GoToTarget()
        {
            //switch (target)
            //{
            //    case Target.MenuScreen:
            //        break;
            //    case Target.Scene:
            //        break;
            //}
            GoToTargetMenuScreen();
            //GoToTargetScene();
        }

        private void GoToTargetMenuScreen()
        {
            MenuManager menu = GetMenuManager();
            if (menu == null) return;
            if (IsBackButton)
            {
                MenuManager.GoToPreviousScreen();
            }
            else
            {
                if (TargetMenuScreen == null)
                    Debug.LogError("Target of button has not been set", gameObject);
                else
                    MenuManager.GoToScreen(TargetMenuScreen);
            }
        }

        //private void GoToTargetScene()
        //{
        //    GameManager.Instance.SceneLoad(TargetScene.name);
        //}

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (TargetMenuScreen != null)
            {
                Gizmos.DrawLine(transform.position, TargetMenuScreen.transform.position);
            }
        }
#endif
    }
}
