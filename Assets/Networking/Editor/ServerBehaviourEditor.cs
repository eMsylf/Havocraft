using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ServerBehaviour))]
[CanEditMultipleObjects]
public class ServerBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var targetScript = (ServerBehaviour)target;
        if (targetScript.m_Driver.IsCreated)
        {
            if (GUILayout.Button("Stop server"))
            {
                targetScript.StopServer();
            }
        }
        else
        {
            if (GUILayout.Button("Start server"))
            {
                targetScript.StartServer();
            }
        }
    }
}
