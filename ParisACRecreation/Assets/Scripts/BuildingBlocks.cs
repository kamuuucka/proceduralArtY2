using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BuildingBlocks : MonoBehaviour
{
    [SerializeField] private GameObject groundWall;
    [SerializeField] private GameObject groundCorner;
    [SerializeField] private GameObject floorWall;
    [SerializeField] private GameObject floorCorner;
    [SerializeField] private GameObject roofCorner;
    [SerializeField] private List<GameObject> buildingRoofs;
    [Range(1,3)]
    [SerializeField] private int roofType;

    [Range(2, 10)] [SerializeField] private int length;
    [Range(2, 10)] [SerializeField] private int width;
    [Range(1, 10)] [SerializeField] private int floors;

    private Vector3 _lastPiecePosition;
    private List<Vector3> _corners = new List<Vector3>();
    private List<GameObject> _children = new List<GameObject>();

    private void Awake()
    {
        Bounds gWallBounds = groundWall.GetComponent<Renderer>().bounds;
        
        Debug.Log(gWallBounds.size.y);
        Debug.Log(gWallBounds.size.x);
        Debug.Log(gWallBounds.size.z);
    }

    public void Build()
    {
        _corners.Clear();

        for (int i = 0; i < 4; i++)
        {
            GameObject corner = Instantiate(groundCorner, transform);
            corner.name = $"Corner{i}";
            corner.transform.Rotate(Vector3.up, 90 - 90 * i);
            Vector3 newPosition = Vector3.zero;
            Vector3 thisPosition = transform.position;
            switch (i)
            {
                case 0:
                    newPosition = thisPosition;
                    break;
                case 1:
                    newPosition = new Vector3(thisPosition.x + 5 * (length - 1), thisPosition.y, thisPosition.z);
                    break;
                case 2:
                    newPosition = new Vector3(thisPosition.x + 5 * (length - 1), thisPosition.y,
                        thisPosition.z + 5 * (width - 1));
                    break;
                case 3:
                    newPosition = new Vector3(thisPosition.x, thisPosition.y, thisPosition.z + 5 * (width - 1));
                    break;
            }

            corner.transform.position = newPosition;
            _corners.Add(newPosition);
            _children.Add(corner);
        }

        //FRONT & BACK WALLS
        for (int i = 1; i <= length - 2; i++)
        {
            GameObject frontWall = Instantiate(groundWall, transform);
            frontWall.name = $"Front{i}";
            frontWall.transform.position = new Vector3(_corners[0].x + 5 * i, _corners[0].y, _corners[0].z);
            _children.Add(frontWall);

            GameObject backWall = Instantiate(groundWall, transform);
            backWall.name = $"Back{i}";
            backWall.transform.position = new Vector3(_corners[2].x - 5 * i, _corners[0].y, _corners[2].z);
            _children.Add(backWall);
        }

        //SIDE WALLS
        for (int i = 1; i <= width - 2; i++)
        {
            GameObject rightWall = Instantiate(groundWall, transform);
            rightWall.name = $"Right{i}";
            rightWall.transform.position = new Vector3(_corners[1].x, _corners[0].y, _corners[1].z + 5 * i);
            rightWall.transform.Rotate(Vector3.up, 90);
            _children.Add(rightWall);

            GameObject leftWall = Instantiate(groundWall, transform);
            leftWall.name = $"Left{i}";
            leftWall.transform.position = new Vector3(_corners[0].x, _corners[0].y, _corners[0].z + 5 * i);
            leftWall.transform.Rotate(Vector3.up, 90);
            _children.Add(leftWall);
        }

        //SECOND FLOOR
        if (floors > 1)
        {
            for (int f = 0; f < floors-1; f++)
            {
                for (int i = 0; i < 4; i++)
                {
                    GameObject corner = Instantiate(floorCorner, transform);
                    corner.name = $"Corner{i}";
                    corner.transform.Rotate(Vector3.up, 90 - 90 * i);
                    Vector3 newPosition = Vector3.zero;
                    Vector3 thisPosition = transform.position;
                    switch (i)
                    {
                        case 0:
                            newPosition = new Vector3(thisPosition.x, thisPosition.y+3.5f+3*f,
                                thisPosition.z);;
                            break;
                        case 1:
                            newPosition = new Vector3(thisPosition.x + 5 * (length - 1), thisPosition.y+3.5f+3*f,
                                thisPosition.z);
                            break;
                        case 2:
                            newPosition = new Vector3(thisPosition.x + 5 * (length - 1), thisPosition.y+3.5f+3*f,
                                thisPosition.z + 5 * (width - 1));
                            break;
                        case 3:
                            newPosition = new Vector3(thisPosition.x, thisPosition.y+3.5f+3*f, thisPosition.z + 5 * (width - 1));
                            break;
                    }

                    corner.transform.position = newPosition;
                    _children.Add(corner);
                }

                //FRONT AND BACK
                for (int i = 1; i <= length - 2; i++)
                {
                    GameObject frontWall = Instantiate(floorWall, transform);
                    frontWall.name = $"Front{i}";
                    frontWall.transform.position = new Vector3(_corners[0].x + 5 * i, _corners[0].y +3.5f+3*f, _corners[0].z);

                    GameObject backWall = Instantiate(floorWall, transform);
                    backWall.name = $"Back{i}";
                    backWall.transform.position = new Vector3(_corners[2].x - 5 * i, _corners[0].y +3.5f+3*f, _corners[2].z);
                    _children.Add(frontWall);
                    _children.Add(backWall);
                }

                //SIDE 
                for (int i = 1; i <= width - 2; i++)
                {
                    GameObject rightWall = Instantiate(floorWall, transform);
                    rightWall.name = $"Right{i}";
                    rightWall.transform.position = new Vector3(_corners[1].x, _corners[0].y +3.5f+3*f, _corners[1].z + 5 * i);
                    rightWall.transform.Rotate(Vector3.up, 90);

                    GameObject leftWall = Instantiate(floorWall, transform);
                    leftWall.name = $"Left{i}";
                    leftWall.transform.position = new Vector3(_corners[0].x, _corners[0].y +3.5f+3*f, _corners[0].z + 5 * i);
                    leftWall.transform.Rotate(Vector3.up, 90);
                    
                    _children.Add(rightWall);
                    _children.Add(leftWall);
                }
            }
        }


        //ROOFS
        for (int i = 0; i < 4; i++)
        {
            GameObject corner = Instantiate(roofCorner, transform);
            corner.name = $"Corner{i}";
            corner.transform.Rotate(Vector3.up, 90 - 90 * i);
            Vector3 newPosition = Vector3.zero;
            Vector3 thisPosition = transform.position;
            switch (i)
            {
                case 0:
                    newPosition = new Vector3(thisPosition.x, thisPosition.y+2+3*(floors-1),
                        thisPosition.z);;
                    break;
                case 1:
                    newPosition = new Vector3(thisPosition.x + 5 * (length - 1), thisPosition.y+2+3*(floors-1),
                        thisPosition.z);
                    break;
                case 2:
                    newPosition = new Vector3(thisPosition.x + 5 * (length - 1), thisPosition.y+2+3*(floors-1),
                        thisPosition.z + 5 * (width - 1));
                    break;
                case 3:
                    newPosition = new Vector3(thisPosition.x, thisPosition.y+2+3*(floors-1), thisPosition.z + 5 * (width - 1));
                    break;
            }

            corner.transform.position = newPosition;
            _children.Add(corner);
        }
        for (int i = 1; i <= length-2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                GameObject roof = Instantiate(buildingRoofs[roofType-1], transform);
                roof.name = $"Roof{i}{j}";
                int changeDirection = j == 0 ? 1 : -1;
                roof.transform.position = new Vector3(_corners[j*2].x + (5 * i) * changeDirection, _corners[0].y+2+3*(floors-1), _corners[j*2].z);
                _children.Add(roof);
            }
        }
        for (int i = 1; i <= width-2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                GameObject roof = Instantiate(buildingRoofs[roofType-1], transform);
                roof.name = $"Roof{i}{j}";
                roof.transform.position = new Vector3(_corners[j].x, _corners[0].y+2 + 3 * (floors - 1), _corners[j].z + 5 * i);
                roof.transform.Rotate(Vector3.up, 90);
                _children.Add(roof);
            }
        }
    }

    public void Reset()
    {
        foreach (var child in _children)
        {
            
                    DestroyImmediate(child);
                
            
        }
    }
}