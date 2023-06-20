using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingsGenerator))]
public class BuildingsGeneratorEditor : Editor
{
    private BuildingsGenerator _generator;
    private List<GameObject> _buildings;

    private void OnEnable()
    {
        _generator = (BuildingsGenerator)target;
        _buildings = _generator.GetBuildings();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Create"))
        {
            _generator.AddBuilding();
        }

        if (_buildings.Count != 0)
        {
            for (var index = 0; index < _buildings.Count; index++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Building " + (index + 1));
                if (GUILayout.Button("Delete"))
                {
                    _generator.RemoveBuilding(_buildings[index]);
                }

                GUILayout.EndHorizontal();
            }
        }
    }
}