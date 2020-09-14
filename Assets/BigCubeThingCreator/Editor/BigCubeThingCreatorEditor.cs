using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BigCubeThingCreator))]
public class BigCubeThingCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BigCubeThingCreator targetScript = (BigCubeThingCreator)target;

        base.OnInspectorGUI();        

        if (GUILayout.Button("Create new multicube"))
        {
            if (EditorUtility.DisplayDialog("Warning", "This will throw away all currently placed cubes under this parent. Continue?", "Continue", "Cancel"))
            {
                //Undo.RecordObjects(targetScript.SpawnedCubes.ToArray(), "Create new multicube"); // Causes issues because some spawnedcubes can be null
                targetScript.NewMulticube();
            }
        }

        if (GUILayout.Button("Combine mesh"))
        {
            //if (EditorUtility.DisplayDialog("Warning", "This will finalize your mesh. Continue?", "Continue", "Cancel"))
                targetScript.CombineMesh();
        }

        if (GUILayout.Button("Uncombine mesh"))
        {
            targetScript.CombineMeshUndo();
        }
    }
}
