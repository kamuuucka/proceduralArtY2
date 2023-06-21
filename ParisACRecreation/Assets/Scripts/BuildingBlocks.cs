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

    [Range(2, 10)] [SerializeField] private int length;
    [Range(2, 10)] [SerializeField] private int width;
    [Range(1, 10)] [SerializeField] private int floors;

    public int Floors => floors;
    public bool AdvancedSettings => _advancedSettings;

    public List<GameObject> GroundWalls => groundWalls;
    public List<GameObject> GroundCorners => groundCorners;
    public List<GameObject> FloorWalls => floorWalls;
    public List<GameObject> FloorCorners => floorCorners;
    public List<GameObject> RoofCorners => roofCorners;
    public List<GameObject> BuildingRoofs => buildingRoofs;

    private Vector3 _lastPiecePosition;
    private readonly List<Vector3> _corners = new List<Vector3>();
    private readonly List<GameObject> _children = new List<GameObject>();
    private List<GameObject> _firstFloorCopy;
    private readonly Dictionary<GameObject, int> _wallsAndFloors = new Dictionary<GameObject, int>();

    public GameObject selectedRoof;
    public GameObject selectedWallGround;
    public GameObject selectedWallFFloor;
    public GameObject selectedWallFloor;

    public enum BlockType
    {
        GroundWall, FirstFloor, OtherFloors, Roof
    }

    private BlockType _block;
    private bool _advancedSettings;

    public void SetBlock(int blockNumber, BlockType blockType)
    {
        switch (blockType)
        {
            case BlockType.GroundWall:
                selectedWallGround = groundWalls[blockNumber];
                break;
            case BlockType.FirstFloor:
                selectedWallFFloor = _firstFloorCopy[blockNumber];
                break;
            case BlockType.OtherFloors:
                selectedWallFloor = floorWalls[blockNumber];
                break;
            case BlockType.Roof:
                selectedRoof = buildingRoofs[blockNumber];
                break;
        }
    }

    public void RemoveBlock(BlockType blockType)
    {
        switch (blockType)
        {
            case BlockType.GroundWall:
                selectedWallGround = null;
                break;
            case BlockType.FirstFloor:
                selectedWallFFloor = null;
                break;
            case BlockType.OtherFloors:
                selectedWallFloor = null;
                break;
            case BlockType.Roof:
                selectedRoof = null;
                break;
        }
    }

    public void Build()
    {
        _corners.Clear();
        _wallsAndFloors.Clear();
        _firstFloorCopy = floorWalls;
        GenerateGround();

        //SECOND FLOOR
        if (floors > 1)
        {
            for (int f = 0; f < floors-1; f++)
            {
                GenerateFloor(f);
            }
        }


        //ROOFS
        GenerateRoof(selectedRoof);

        foreach (var child in _children)
        {
            Debug.Log(child);
        }
    }

    public void RegeneratePart(int floor)
    {
        List<GameObject> temporaryChildren = new List<GameObject>();
        foreach (GameObject child in _children)
        {
            if (_wallsAndFloors.ContainsKey(child) && _wallsAndFloors[child] == floor)
            {
                temporaryChildren.Add(child);
            }
        }

        foreach (GameObject child in temporaryChildren)
        {
            _children.Remove(child);
            _wallsAndFloors.Remove(child);
            DestroyImmediate(child);
        }

        if (floor==0) GenerateGround();
        else if (floor==floors+1) GenerateRoof(null);
        else GenerateFloor(floor-1);
    }

    private void GenerateRoof(GameObject roofType)
    {
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
                    newPosition = new Vector3(thisPosition.x, thisPosition.y + 2 + 3 * (floors - 1),
                        thisPosition.z);
                    ;
                    break;
                case 1:
                    newPosition = new Vector3(thisPosition.x + 5 * (length - 1), thisPosition.y + 2 + 3 * (floors - 1),
                        thisPosition.z);
                    break;
                case 2:
                    newPosition = new Vector3(thisPosition.x + 5 * (length - 1), thisPosition.y + 2 + 3 * (floors - 1),
                        thisPosition.z + 5 * (width - 1));
                    break;
                case 3:
                    newPosition = new Vector3(thisPosition.x, thisPosition.y + 2 + 3 * (floors - 1),
                        thisPosition.z + 5 * (width - 1));
                    break;
            }

            corner.transform.position = newPosition;
            _children.Add(corner);
            _wallsAndFloors.Add(corner,floors+1);
        }

        int randomRoof = 0;
        
        for (int i = 1; i <= length - 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (selectedRoof == null)
                {
                    randomRoof = Random.Range(0, buildingRoofs.Count);
                
                }
                else
                {
                    randomRoof = buildingRoofs.IndexOf(selectedRoof);
                }
                GameObject roof = Instantiate(buildingRoofs[randomRoof], transform);
                roof.name = $"Roof{i}{j}";
                int changeDirection = j == 0 ? 1 : -1;
                roof.transform.position = new Vector3(_corners[j * 2].x + (5 * i) * changeDirection,
                    _corners[0].y + 2 + 3 * (floors - 1), _corners[j * 2].z);
                int rotation = j == 0 ? 0 : 180;
                roof.transform.Rotate(Vector3.up, rotation);
                _children.Add(roof);
                _wallsAndFloors.Add(roof,floors+1);
            }
        }

        for (int i = 1; i <= width - 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (selectedRoof == null)
                {
                    randomRoof = Random.Range(0, buildingRoofs.Count);
                
                } else
                {
                    randomRoof = buildingRoofs.IndexOf(selectedRoof);
                }
                GameObject roof = Instantiate(buildingRoofs[randomRoof], transform);
                roof.name = $"Roof{i}{j}";
                roof.transform.position =
                    new Vector3(_corners[j].x, _corners[0].y + 2 + 3 * (floors - 1), _corners[j].z + 5 * i);
                int rotation = j == 0 ? 90 : -90;
                roof.transform.Rotate(Vector3.up, rotation);
                _children.Add(roof);
                _wallsAndFloors.Add(roof,floors+1);
            }
        }
    }

    private void GenerateFloor(int f)
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
                    newPosition = new Vector3(thisPosition.x, thisPosition.y + 3.5f + 3 * f,
                        thisPosition.z);
                    ;
                    break;
                case 1:
                    newPosition = new Vector3(thisPosition.x + 5 * (length - 1), thisPosition.y + 3.5f + 3 * f,
                        thisPosition.z);
                    break;
                case 2:
                    newPosition = new Vector3(thisPosition.x + 5 * (length - 1), thisPosition.y + 3.5f + 3 * f,
                        thisPosition.z + 5 * (width - 1));
                    break;
                case 3:
                    newPosition = new Vector3(thisPosition.x, thisPosition.y + 3.5f + 3 * f,
                        thisPosition.z + 5 * (width - 1));
                    break;
            }

            corner.transform.position = newPosition;
            _children.Add(corner);
            _wallsAndFloors.Add(corner,f+1);
        }

        int randomFront = 0;
        int randomBack = 0;
        //FRONT AND BACK
        for (int i = 1; i <= length - 2; i++)
        {
            randomFront = Random.Range(0, floorWalls.Count);
            randomBack = Random.Range(0, floorWalls.Count);
            GameObject frontWall = Instantiate(floorWalls[randomFront], transform);
            frontWall.name = $"Front{i}";
            frontWall.transform.position = new Vector3(_corners[0].x + 5 * i, _corners[0].y + 3.5f + 3 * f, _corners[0].z);

            GameObject backWall = Instantiate(floorWalls[randomBack], transform);
            backWall.name = $"Back{i}";
            backWall.transform.position = new Vector3(_corners[2].x - 5 * i, _corners[0].y + 3.5f + 3 * f, _corners[2].z);
            backWall.transform.Rotate(Vector3.up, 180);
            _children.Add(frontWall);
            _children.Add(backWall);
            
            _wallsAndFloors.Add(frontWall,f+1);
            _wallsAndFloors.Add(backWall,f+1);
        }

        //SIDE 
        for (int i = 1; i <= width - 2; i++)
        {
            int randomRight = Random.Range(0, floorWalls.Count);
            int randomLeft = Random.Range(0, floorWalls.Count);
            GameObject rightWall = Instantiate(floorWalls[randomRight], transform);
            rightWall.name = $"Right{i}";
            rightWall.transform.position = new Vector3(_corners[1].x, _corners[0].y + 3.5f + 3 * f, _corners[1].z + 5 * i);
            rightWall.transform.Rotate(Vector3.up, -90);

            GameObject leftWall = Instantiate(floorWalls[randomLeft], transform);
            leftWall.name = $"Left{i}";
            leftWall.transform.position = new Vector3(_corners[0].x, _corners[0].y + 3.5f + 3 * f, _corners[0].z + 5 * i);
            leftWall.transform.Rotate(Vector3.up, 90);

            _children.Add(rightWall);
            _children.Add(leftWall);
            
            _wallsAndFloors.Add(rightWall,f+1);
            _wallsAndFloors.Add(leftWall,f+1);
        }
    }

    private void GenerateGround()
    {
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
            _wallsAndFloors.Add(corner,0);
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
            _wallsAndFloors.Add(frontWall,0);

            GameObject backWall = Instantiate(groundWalls[randomBack], transform);
            backWall.name = $"Back{i}";
            backWall.transform.position = new Vector3(_corners[2].x - 5 * i, _corners[0].y, _corners[2].z);
            backWall.transform.Rotate(Vector3.up, 180);
            _children.Add(backWall);
            _wallsAndFloors.Add(backWall,0);
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
            
            _wallsAndFloors.Add(rightWall,0);
            _wallsAndFloors.Add(leftWall,0);
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