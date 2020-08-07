using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BobJeltes
{
    [RequireComponent(typeof(Button))]
    public class MenuButton : MonoBehaviour
    {
        private MenuManager MenuManager;
        public MenuScreen Target;
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
            MenuManager menu = GetMenuManager();
            if (menu == null) return;
            if (IsBackButton)
            {
                MenuManager.GoToPreviousScreen();
            }
            else
            {
                if (Target == null)
                    Debug.LogError("Target of button has not been set", gameObject);
                else
                    MenuManager.GoToScreen(Target);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (Target != null)
            {
                Gizmos.DrawLine(transform.position, Target.transform.position);
            }
        }
    }
}
