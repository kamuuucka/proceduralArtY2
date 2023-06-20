using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BuildingBlocks : MonoBehaviour
{
    [SerializeField] private List<GameObject> groundWalls;
    [SerializeField] private List<GameObject> groundCorners;
    [SerializeField] private List<GameObject> floorWalls;
    [SerializeField] private List<GameObject> floorCorners;
    [SerializeField] private List<GameObject> roofCorners;
    [SerializeField] private List<GameObject> buildingRoofs;
    [Range(1,3)]
    [SerializeField] private int roofType;

    [Range(2, 10)] [SerializeField] private int length;
    [Range(2, 10)] [SerializeField] private int width;
    [Range(1, 10)] [SerializeField] private int floors;

    private Vector3 _lastPiecePosition;
    private List<Vector3> _corners = new List<Vector3>();
    private List<GameObject> _children = new List<GameObject>();

    public void Build()
    {
        _corners.Clear();

        for (int i = 0; i < 4; i++)
        {
            int randomPiece = Random.Range(0, groundCorners.Count);
            GameObject corner = Instantiate(groundCorners[randomPiece], transform);
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
            int randomFront = Random.Range(0, groundWalls.Count);
            int randomBack = Random.Range(0, groundWalls.Count);
            GameObject frontWall = Instantiate(groundWalls[randomFront], transform);
            frontWall.name = $"Front{i}";
            frontWall.transform.position = new Vector3(_corners[0].x + 5 * i, _corners[0].y, _corners[0].z);
            _children.Add(frontWall);

            GameObject backWall = Instantiate(groundWalls[randomBack], transform);
            backWall.name = $"Back{i}";
            backWall.transform.position = new Vector3(_corners[2].x - 5 * i, _corners[0].y, _corners[2].z);
            backWall.transform.Rotate(Vector3.up,180);
            _children.Add(backWall);
        }

        //SIDE WALLS
        for (int i = 1; i <= width - 2; i++)
        {
            int randomRight = Random.Range(0, groundWalls.Count);
            int randomLeft = Random.Range(0, groundWalls.Count);
            GameObject rightWall = Instantiate(groundWalls[randomRight], transform);
            rightWall.name = $"Right{i}";
            rightWall.transform.position = new Vector3(_corners[1].x, _corners[0].y, _corners[1].z + 5 * i);
            rightWall.transform.Rotate(Vector3.up, -90);
            _children.Add(rightWall);

            GameObject leftWall = Instantiate(groundWalls[randomLeft], transform);
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
                    int randomCorner = Random.Range(0, floorCorners.Count);
                    GameObject corner = Instantiate(floorCorners[randomCorner], transform);
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
                    int randomFront = Random.Range(0, floorWalls.Count);
                    int randomBack = Random.Range(0, floorWalls.Count);
                    GameObject frontWall = Instantiate(floorWalls[randomFront], transform);
                    frontWall.name = $"Front{i}";
                    frontWall.transform.position = new Vector3(_corners[0].x + 5 * i, _corners[0].y +3.5f+3*f, _corners[0].z);

                    GameObject backWall = Instantiate(floorWalls[randomBack], transform);
                    backWall.name = $"Back{i}";
                    backWall.transform.position = new Vector3(_corners[2].x - 5 * i, _corners[0].y +3.5f+3*f, _corners[2].z);
                    backWall.transform.Rotate(Vector3.up,180);
                    _children.Add(frontWall);
                    _children.Add(backWall);
                }

                //SIDE 
                for (int i = 1; i <= width - 2; i++)
                {
                    int randomRight = Random.Range(0, floorWalls.Count);
                    int randomLeft = Random.Range(0, floorWalls.Count);
                    GameObject rightWall = Instantiate(floorWalls[randomRight], transform);
                    rightWall.name = $"Right{i}";
                    rightWall.transform.position = new Vector3(_corners[1].x, _corners[0].y +3.5f+3*f, _corners[1].z + 5 * i);
                    rightWall.transform.Rotate(Vector3.up, -90);

                    GameObject leftWall = Instantiate(floorWalls[randomLeft], transform);
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
            int randomRoofCorner = Random.Range(0, roofCorners.Count);
            GameObject corner = Instantiate(roofCorners[randomRoofCorner], transform);
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
                int randomRoof = Random.Range(0, buildingRoofs.Count);
                GameObject roof = Instantiate(buildingRoofs[randomRoof], transform);
                roof.name = $"Roof{i}{j}";
                int changeDirection = j == 0 ? 1 : -1;
                roof.transform.position = new Vector3(_corners[j*2].x + (5 * i) * changeDirection, _corners[0].y+2+3*(floors-1), _corners[j*2].z);
                int rotation = j == 0 ? 0 : 180;
                roof.transform.Rotate(Vector3.up, rotation);
                _children.Add(roof);
            }
        }
        for (int i = 1; i <= width-2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int randomRoof = Random.Range(0, buildingRoofs.Count);
                GameObject roof = Instantiate(buildingRoofs[randomRoof], transform);
                roof.name = $"Roof{i}{j}";
                roof.transform.position = new Vector3(_corners[j].x, _corners[0].y+2 + 3 * (floors - 1), _corners[j].z + 5 * i);
                int rotation = j == 0 ? 90 : -90;
                roof.transform.Rotate(Vector3.up, rotation);
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