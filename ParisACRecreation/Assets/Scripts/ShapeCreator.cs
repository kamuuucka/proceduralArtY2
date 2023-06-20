using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ShapeCreator : MonoBehaviour
{
    [HideInInspector]
    public List<Vector3> points = new List<Vector3>();


    public float handleRadius = 0.5f;
    public float lineThickness = 4;
    public float height = 10;
    public float roofHeight = 10;
    public float roofOffset = 6;

    public void Apply()
    {
        MeshCreator meshCreator = GetComponent<MeshCreator>();
        if (meshCreator != null)
        {
            meshCreator.RecalculateMesh();
        }
    }

    public void Clear()
    {
        if (gameObject.GetComponent<MeshFilter>())
        {
            Mesh mesh = gameObject.GetComponent <MeshFilter>().sharedMesh;
            DestroyImmediate(mesh, true);
        }
    }
}