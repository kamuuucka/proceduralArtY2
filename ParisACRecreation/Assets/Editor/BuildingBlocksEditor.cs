using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(BuildingBlocks))]
public class BuildingBlocksEditor : Editor
{
    private BuildingBlocks _building;
    private bool _showRegeneration;
    private bool _showAdvanced;
    private bool _setGroundWalls;
    private bool _setFirstFloor;
    private bool _setOtherFloors;
    private bool _setRoof;

    private int _selectedWall;
    private int _selectedFloor;
    private int _selectedRest;
    private int _selectedRoof;
    
    private readonly List<string> _gWNamesList = new List<string>();
    private readonly List<string> _rFNamesList = new List<string>();
    private readonly List<string> _rNamesList = new List<string>();

    private string[] _gWNames;
    private string[] _fFNames;
    private string[] _rNames;


    private void OnEnable()
    {
        _building = (BuildingBlocks)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        foreach (var child in _building.GroundWalls)
        {
            _gWNamesList.Add(child.name);
        }
        foreach (var child in _building.FloorWalls)
        {
            _rFNamesList.Add(child.name);
        }
        foreach (var child in _building.BuildingRoofs)
        {
            _rNamesList.Add(child.name);
        }

        _gWNames = _gWNamesList.ToArray();
        _fFNames = _rFNamesList.ToArray();
        _rNames = _rNamesList.ToArray();

        if (GUILayout.Button("Create"))
        {
            _building.Reset();
            _building.Build();
        }

        if (GUILayout.Button("Delete"))
        {
            _building.Reset();
        }

        if (GUILayout.Button("Advanced settings"))
        {
            ShowAdvancedSettings();
        }
        
        if (_showAdvanced)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox("If you see this message, you are setting up the assets on the floors manually.\n" +
                                    "Click on the part of the building you want to set up and choose the asset from the list.", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            _setGroundWalls = EditorGUILayout.Toggle("Set Ground Walls", _setGroundWalls);
            _selectedWall = EditorGUILayout.Popup(_selectedWall, _gWNames);
            EditorGUILayout.EndHorizontal();
            if (_building.Floors > 1)
            {
                EditorGUILayout.BeginHorizontal();
                _setFirstFloor = EditorGUILayout.Toggle("Set 1st Floor", _setFirstFloor);
                _selectedFloor = EditorGUILayout.Popup(_selectedFloor, _fFNames);
                EditorGUILayout.EndHorizontal();
            }
            
            if (_building.Floors > 2)
            {EditorGUILayout.BeginHorizontal();
                _setOtherFloors = EditorGUILayout.Toggle("Set Other Floors", _setOtherFloors);
                _selectedRest = EditorGUILayout.Popup(_selectedRest, _fFNames);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            _setRoof = EditorGUILayout.Toggle("Set Roof", _setRoof);
            _selectedRoof = EditorGUILayout.Popup(_selectedRoof, _rNames);
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }
        else
        {
            _setRoof = false;
            _setOtherFloors = false;
            _setFirstFloor = false;
            _setGroundWalls = false;
        }

        if (GUILayout.Button("Partial Regeneration"))
        {
            ShowRegeneration();
        }

        if (_showRegeneration)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox("If you see this message, you can regenerate parts of the building.\n" +
                                    "Click on the button with the section you want to regenerate.", MessageType.Info);

            if (GUILayout.Button("Regenerate ground"))
            {
                _building.RegeneratePart(0);
            }

            for (int i = 0; i < _building.Floors - 1; i++)
            {
                EditorGUI.indentLevel++;
                if (GUILayout.Button($"Regenerate {i + 1} Floor"))
                {
                    _building.RegeneratePart(i + 1);
                }
            }

            if (GUILayout.Button("Regenerate Roof"))
            {
                _building.RegeneratePart(_building.Floors + 1);
            }

            EditorGUI.indentLevel--;
        }

        

        if (_setRoof)
        {
            _building.SetBlock(_selectedRoof, BuildingBlocks.BlockType.Roof);
        }
        else
        {
            _building.RemoveBlock(BuildingBlocks.BlockType.Roof);
        }
        if (_setFirstFloor)
        {
            _building.SetBlock(_selectedFloor, BuildingBlocks.BlockType.FirstFloor);
        }
        else
        {
            _building.RemoveBlock(BuildingBlocks.BlockType.FirstFloor);
        }
        if (_setOtherFloors)
        {
            _building.SetBlock(_selectedRest, BuildingBlocks.BlockType.OtherFloors);
        }
        else
        {
            _building.RemoveBlock(BuildingBlocks.BlockType.OtherFloors);
        }
        if (_setGroundWalls)
        {
            _building.SetBlock(_selectedWall, BuildingBlocks.BlockType.GroundWall);
        }
        else
        {
            _building.RemoveBlock(BuildingBlocks.BlockType.GroundWall);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(_building);
            EditorSceneManager.MarkSceneDirty(_building.gameObject.scene);
        }
    }

    private void ShowRegeneration()
    {
        _showRegeneration = !_showRegeneration;
    }

    private void ShowAdvancedSettings()
    {
        _showAdvanced = !_showAdvanced;
    }

   

   
}