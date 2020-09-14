using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MeshCombiner targetScript = (MeshCombiner)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Combine mesh"))
        {
            Undo.RecordObject(targetScript, "Combine mesh");
            targetScript.CombineMesh();
        }
    }
}
