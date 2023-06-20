using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutoUV))]
public class AutoUVEditor : Editor
{
    public override void OnInspectorGUI() {
        AutoUV targetUv = (AutoUV)target;

        if (GUILayout.Button("Recalculate UVs")) {
            targetUv.UpdateUvs();
            EditorUtility.SetDirty(targetUv); // otherwise the new mesh won't be saved into the scene!
        }
        DrawDefaultInspector();
    }
}