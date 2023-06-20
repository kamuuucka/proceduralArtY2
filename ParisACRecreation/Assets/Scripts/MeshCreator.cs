using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public abstract class MeshCreator : MonoBehaviour
{
    public abstract void RecalculateMesh();

    protected void ReplaceMesh(Mesh newMesh, bool changeSharedMesh = false)
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (!changeSharedMesh)
        {
            mf.sharedMesh = newMesh;
        }
        else
        {
            mf.sharedMesh.Clear();
            mf.sharedMesh.vertices = newMesh.vertices;
            mf.sharedMesh.uv = newMesh.uv;
            mf.sharedMesh.triangles = newMesh.triangles;
            mf.sharedMesh.normals = newMesh.normals;
            mf.sharedMesh.tangents = newMesh.tangents;
        }
    }

    public abstract void RemoveMesh(MeshBuilder builder);
}