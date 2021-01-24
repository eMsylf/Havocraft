using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ClientBehaviour))]
public class ClientBehaviourEditor : Editor
{
    bool forceReconnection;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); 
        var targetScript = (ClientBehaviour)target;
        //GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Connect to server"))
            {
                targetScript.ConnectToServer(forceReconnection);
            }
            forceReconnection = GUILayout.Toggle(forceReconnection, "Force reconnection");
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Report connection state"))
        {
            targetScript.ReportConnectionState();
        }

        GUILayout.BeginHorizontal();
        {
            targetScript.value = (uint)EditorGUILayout.IntField("Value", (int)targetScript.value);

            EditorGUI.BeginDisabledGroup(!targetScript.m_Connection.IsCreated || (targetScript.m_Connection.GetState(targetScript.m_Driver) == Unity.Networking.Transport.NetworkConnection.State.Connecting));
            if (GUILayout.Button("Send value to server"))
            {
                targetScript.SendValueToServer();
            }
            EditorGUI.EndDisabledGroup();
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Disconnect from server"))
        {
            targetScript.Disconnect(BobJeltes.DisconnectionReason.ManualClientDisconnection);
        }
    }
}
