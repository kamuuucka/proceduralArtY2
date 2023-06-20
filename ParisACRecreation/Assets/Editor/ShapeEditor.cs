using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeCreator))]
public class ShapeEditor : Editor
{
    private ShapeCreator _creator;
    private bool _repaint;

    private int _currentPointActive = -1;
    private bool _pointSelected;
    private bool _pointHovered;

    private void OnEnable()
    {
        _creator = (ShapeCreator)target;
        Tools.hidden = true;
    }

    private void OnDisable()
    {
        Tools.hidden = false;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        GUI.enabled = true;
        
        

        if (GUILayout.Button("Build Mesh"))
        {
            _creator.Apply();
        }

        if (GUILayout.Button("Delete Mesh"))
        {
            _creator.Clear();
        }
        if (GUILayout.Button("Delete All"))
        {
            _creator.points.Clear();
            _creator.Clear();
        }
        GUILayout.EndHorizontal();
        
        EditorGUILayout.HelpBox("Remember to add 2 materials on the building. Otherwise roof won't generate!", MessageType.Error);
    }
    
    private void OnSceneGUI()
    {
        Event guiEvent = Event.current;

        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneHeight = _creator.transform.position.y;
        float distanceToDraw = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.y;
        Vector3 mousePosition = mouseRay.GetPoint(distanceToDraw);

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            MouseLeftButtonDown(mousePosition);
        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.Shift)
        {
            MouseLeftButtonDownShift(mousePosition);
        }

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
        {
            MouseLeftButtonUp();
        }

        if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            MouseDrag(mousePosition);
        }

        DrawPoints();
        MouseOverPoint(mousePosition);

        if (_repaint)
        {
            RedrawPoints();
        }


        if (guiEvent.type == EventType.Layout)
        {
            //Don't deselect the object
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
    }

    /// <summary>
    /// Redraw points after a change so the whole tool works much smoother
    /// </summary>
    private void RedrawPoints()
    {
        HandleUtility.Repaint();
        _repaint = false;
    }

    /// <summary>
    /// Create a point on the current mouse position
    /// </summary>
    /// <param name="mousePosition"> Current mouse position in the scene </param>
    private void CreatePoint(Vector3 mousePosition)
    {
        Undo.RecordObject(_creator, "Add point to the creator");
        _creator.points.Add(mousePosition);
        _currentPointActive = _creator.points.Count - 1;
        _repaint = true;
        _pointSelected = true;
        _pointHovered = true;
    }

    /// <summary>
    /// Delete a point on the current mouse position
    /// </summary>
    private void DeletePoint()
    {
        Undo.RecordObject(_creator, "Delete point in the creator");
        _creator.points.RemoveAt(_currentPointActive);
        _pointSelected = false;
        _pointHovered = false;
        _repaint = true;
    }

    /// <summary>
    /// Draw points in the scene. The red dots are points, the blue lines connect them
    /// </summary>
    private void DrawPoints()
    {
        for (int i = 0; i < _creator.points.Count; ++i)
        {
            Vector3 currentPoint = _creator.points[i];
            Vector3 nextPoint = i + 1 == _creator.points.Count ? _creator.points[0] : _creator.points[i + 1];

            Handles.color = Color.blue;
            Handles.DrawLine(currentPoint, nextPoint, _creator.lineThickness);

            if (i == _currentPointActive)
            {
                Handles.color = Color.white;
            }
            else
            {
                Handles.color = Color.red;
            }

            Handles.DrawSolidDisc(currentPoint, Vector3.up, _creator.handleRadius);
        }
    }

    /// <summary>
    /// Check if mouse is hovering over the point and update the current point index number
    /// </summary>
    /// <param name="mousePosition"> Current position of the mouse</param>
    private void MouseOverPoint(Vector3 mousePosition)
    {
        int pointMouseHoveredIndex = -1;


        for (int i = 0; i < _creator.points.Count; i++)
        {
            if (Vector3.Distance(mousePosition, _creator.points[i]) < _creator.handleRadius)
            {
                pointMouseHoveredIndex = i;
                break;
            }
        }

        if (pointMouseHoveredIndex != _currentPointActive)
        {
            _currentPointActive = pointMouseHoveredIndex;

            if (_currentPointActive != -1)
            {
                _pointHovered = true;
            }
            else
            {
                _pointHovered = false;
            }
        }

        _repaint = true;
    }

    /// <summary>
    /// Delete point if mouse is hovered over, if not - create a point
    /// </summary>
    /// <param name="mousePosition"></param>
    private void MouseLeftButtonDownShift(Vector3 mousePosition)
    {
        if (_pointHovered)
        {
            DeletePoint();
        }
        else
        {
            CreatePoint(mousePosition);
        }
    }

    /// <summary>
    /// If there's no point - creates a point, if there is already a point - selects it
    /// </summary>
    /// <param name="mousePosition"></param>
    private void MouseLeftButtonDown(Vector3 mousePosition)
    {
        if (!_pointHovered)
        {
            CreatePoint(mousePosition);
        }
        else
        {
            _pointSelected = true;
            _pointHovered = true;
            _repaint = true;
        }
    }

    /// <summary>
    /// Deselects point after the button is released
    /// </summary>
    private void MouseLeftButtonUp()
    {
        if (_pointSelected)
        {
            _pointSelected = false;
            _currentPointActive = -1;
            _repaint = true;
        }
    }

    /// <summary>
    /// Drags the point if mouse button is being held
    /// </summary>
    /// <param name="mousePosition"></param>
    private void MouseDrag(Vector3 mousePosition)
    {
        if (_pointSelected)
        {
            Undo.RecordObject(_creator, "Move point in the creator");
            _creator.points[_currentPointActive] = mousePosition;
            _repaint = true;
        }
    }
}