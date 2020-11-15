using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraFollow))]
public class CameraFollowInspector : Editor
{
    public bool LinkOffsets;
    public Vector3 UniversalOffset;
    public override void OnInspectorGUI()
    {
        CameraFollow targetScript = (CameraFollow)target;
        base.OnInspectorGUI();

        //if (GUILayout.Button("Inherit position offset from transform"))
        //{
        //    targetScript.OverrideOffsets();
        //}

        if (targetScript.LivePreview)
        {
            targetScript.SetPositionAndLook();
        }

        LinkOffsets = EditorGUILayout.Toggle("Link Offsets", LinkOffsets);
        if (LinkOffsets)
        {
            UniversalOffset = EditorGUILayout.Vector3Field("Universal offset", UniversalOffset);
            targetScript.LookOffset = UniversalOffset;
            targetScript.PositionOffset = UniversalOffset;
        }
    }
}
