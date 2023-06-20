using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingBlocks))]
public class BuildingBlocksEditor : Editor
{
    private BuildingBlocks _building;

    private void OnEnable()
    {
        _building = (BuildingBlocks)target;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Create"))
        {
            _building.Reset();
            _building.Build();
        }
        if (GUILayout.Button("Delete"))
        {
            _building.Reset();
        }
        
    }
}