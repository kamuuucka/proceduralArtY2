using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BuildingsGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects;
    [SerializeField] private GameObject creatorPrefab;

    

    public void AddBuilding()
    {
        GameObject newBuilding = Instantiate(creatorPrefab, transform);
        newBuilding.name = "Building " + (objects.Count + 1);
        objects.Add(newBuilding);
    }

    public void RemoveBuilding(GameObject building)
    {
        if (building.GetComponent<MeshFilter>())
        {
            Mesh mesh = building.GetComponent <MeshFilter>().sharedMesh;
            DestroyImmediate(mesh, true);
        }
        objects.Remove(building);
        DestroyImmediate(building);
    }

    public List<GameObject> GetBuildings()
    {
        return objects;
    }

}