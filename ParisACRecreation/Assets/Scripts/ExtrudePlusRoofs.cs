using System.Collections.Generic;
using UnityEngine;
using System;

//If you have time, make the if to also be able to draw it clockwise (now it's only counterclockwise)

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class ExtrudePlusRoofs : MeshCreator
{
    [SerializeField] private bool modifySharedMesh;
    private float _height = 10;
    private float _roofHeight = 10;
    private float _roofOffset = 6;
    private float _creatorPositionX;
    private float _creatorPositionZ;
    private float _creatorPositionY;
    private List<int> _triangles;

    public override void RecalculateMesh()
    {
        ShapeCreator creator = GetComponent<ShapeCreator>();

        if (creator == null) return;
        _height = creator.height;
        _roofHeight = creator.roofHeight;
        _roofOffset = creator.roofOffset;
        Vector3 creatorPosition = creator.transform.position;
        _creatorPositionX = creatorPosition.x;
        _creatorPositionZ= creatorPosition.z;
        _creatorPositionY = 0;
        
        List<Vector3> points = creator.points;

        //Make sure that we can actually generate the mesh
        if (points.Count <= 2)
        {
            Debug.LogError("Cannot generate when there are less than 3 vertices!");
            return;
        }
        
        List<Vector3> polygon = new List<Vector3>();
        for (int i = 0; i < points.Count; i++)
        {
            polygon.Add(new Vector3(points[i].x - _creatorPositionX, 0, points[i].z - _creatorPositionZ));
        }
        
        List<int> indices = new List<int>();
        for (int i = 0; i < polygon.Count; i++)
        {
            indices.Add(i);
        }
        
        _triangles = new List<int>();
        
        TriangulatePolygon(_triangles, polygon, indices);

        MeshBuilder builder = new MeshBuilder();
        List<Vector3> roofVertices = new List<Vector3>();

        // Add front face:
        for (int i = 0; i < points.Count; i++)
        {
            builder.AddVertex(new Vector3(points[i].x - _creatorPositionX, _height,
                points[i].z - _creatorPositionZ));
        }

        //Roof vertices
        Vector3 offsetVectorFinal = GenerateOffsetPoint(points, points.Count - 1, 0, 1);
        roofVertices.Add(new Vector3(points[0].x - _creatorPositionX + offsetVectorFinal.x,
            _creatorPositionY + 2 * _height,
            points[0].z - _creatorPositionZ + offsetVectorFinal.z));
        for (int i = 1; i < points.Count - 1; i++)
        {
            offsetVectorFinal = GenerateOffsetPoint(points, i - 1, i, i + 1);
            roofVertices.Add(new Vector3(points[i].x - _creatorPositionX + offsetVectorFinal.x,
                _creatorPositionY + 2 * _height,
                points[i].z - _creatorPositionZ + offsetVectorFinal.z));
        }

        offsetVectorFinal = GenerateOffsetPoint(points, points.Count - 2, points.Count - 1, 0);
        roofVertices.Add(new Vector3(points[^1].x - _creatorPositionX + offsetVectorFinal.x,
            _creatorPositionY + 2* _height,
            points[^1].z - _creatorPositionZ + offsetVectorFinal.z));

        for (int t = 0; t < _triangles.Count; t += 3)
        {
            //INSIDE ROOF
            builder.AddTriangle(_triangles[t], _triangles[t + 1], _triangles[t + 2]);
        }
        
        //Roof vertex
        int m = points.Count;
        for (int i = 0; i < roofVertices.Count; i++)
        {
            builder.AddVertex(new Vector3(roofVertices[i].x, roofVertices[i].y + _roofHeight, roofVertices[i].z));
        }
        
        for (int t = 0; t < _triangles.Count; t += 3)
        {
            builder.AddTriangle(m + _triangles[t], m + _triangles[t + 1], m + _triangles[t + 2], 1);
        }

        for (int i = 0; i < points.Count; i++)
        {
            int j = (i + 1) % points.Count;
            // front vertices:
            int v1 = builder.AddVertex(new Vector3(points[i].x - _creatorPositionX, _creatorPositionY + 2 * _height,
                points[i].z - _creatorPositionZ));
            int v2 = builder.AddVertex(new Vector3(points[j].x - _creatorPositionX, _creatorPositionY + 2 * _height,
                points[j].z - _creatorPositionZ));
            // back vertices:
            int v3 = builder.AddVertex(new Vector3(roofVertices[i].x, roofVertices[i].y + _roofHeight, roofVertices[i].z));
            int v4 = builder.AddVertex(new Vector3(roofVertices[j].x, roofVertices[j].y + _roofHeight, roofVertices[j].z));
            // Add quad:
            builder.AddTriangle(v1, v2, v3, 1);
            builder.AddTriangle(v2, v4, v3, 1);
        }


        // Add back face:
        int n = points.Count * 2;
        for (int i = 0; i < points.Count; i++)
        {
            builder.AddVertex(new Vector3(points[i].x - _creatorPositionX, _creatorPositionY,
                points[i].z - _creatorPositionZ));
        }

        for (int t = 0; t < _triangles.Count; t += 3)
        {
            builder.AddTriangle(n + _triangles[t + 2], n + _triangles[t + 1], n + _triangles[t], 1);
        }

        // Add sides:
        for (int i = 0; i < points.Count; i++)
        {
            int j = (i + 1) % points.Count;
            // front vertices:
            int v1 = builder.AddVertex(new Vector3(points[i].x - _creatorPositionX, _creatorPositionY + 2 * _height,
                points[i].z - _creatorPositionZ));
            int v2 = builder.AddVertex(new Vector3(points[j].x - _creatorPositionX, _creatorPositionY + 2 * _height,
                points[j].z - _creatorPositionZ));
            // back vertices:
            int v3 = builder.AddVertex(new Vector3(points[i].x - _creatorPositionX, _creatorPositionY,
                points[i].z - _creatorPositionZ));
            int v4 = builder.AddVertex(new Vector3(points[j].x - _creatorPositionX, _creatorPositionY,
                points[j].z - _creatorPositionZ));
            // Add quad:
            builder.AddTriangle(v1, v3, v2);
            builder.AddTriangle(v2, v3, v4);
        }

        ReplaceMesh(builder.CreateMesh(), modifySharedMesh);
    }

    public override void RemoveMesh(MeshBuilder builder)
    {
        builder.Clear();
    }

    private Vector3 GenerateOffsetPoint(List<Vector3> points, int firstVector, int middleVector, int lastVector)
    {
        Vector3 offsetVector1 = points[firstVector] - points[middleVector];
        Vector3 offsetVector2 = points[lastVector] - points[middleVector];

        Vector3 middleAngle = offsetVector1 + offsetVector2;
        Vector3 middleAngleNormalized = Vector3.Normalize(middleAngle);

        Vector3 offsetVectorFinal = middleAngleNormalized * _roofOffset;

        int modifier = 1;
        float angle = ((Mathf.Atan2(-offsetVector2.z, -offsetVector2.x) + Mathf.PI) -
                       (Mathf.Atan2(-offsetVector1.z, -offsetVector1.x) + Mathf.PI));
        angle *= 180 / Mathf.PI;
        if (angle < 0)
        {
            angle += 360;
        }

        if (angle > 180)
        {
            modifier = -1;
        }


        return offsetVectorFinal * modifier;
    }

    // *IF* [polygon] respresents a simple polygon (no crossing edges), given in clockwise order, then 
    // this method will return in [triangles] a triangulation of the polygon, using the vertex indices from [indices]
    // If the assumption is not satisfied, the output is undefined.
    void TriangulatePolygon(List<int> triangles, List<Vector3> polygon, List<int> indices)
    {
        for (int i = 0; i < polygon.Count; i++)
        {
            int i2 = (i + 1) % polygon.Count;
            int i3 = (i + 2) % polygon.Count;
            Vector3 u = polygon[i];
            Vector3 v = polygon[i2];
            Vector3 w = polygon[i3];


            // - if not continue the for loop (with the next value for i)

            if (!Clockwise(u, v, w))
            {
                continue;
            }


            // - if not continue the for loop (with the next value for i)
            bool noPointsInsideTriangle = true;
            for (int j = i + 3; j < polygon.Count; j++)
            {
                if (InsideTriangle(u, v, w, polygon[j]))
                {
                    noPointsInsideTriangle = false;
                    break;
                }
            }

            if (!noPointsInsideTriangle)
            {
                continue;
            }


            // (Hint: see the methods below!!! :-)  )

            // Add a triangle on u,v,w:
            triangles.Add(indices[i]);
            triangles.Add(indices[i2]);
            triangles.Add(indices[i3]);

            polygon.RemoveAt(i2); // remove v from point list (keep u and w!)
            indices.RemoveAt(i2); // also remove the corresponding index from the index list
            if (polygon.Count < 3)
                return; // The polygon is now fully triangulated

            // continue with a smaller polygon - restart the for loop:
            i = -1;
        }

        throw new Exception("No suitable triangulation found - is the polygon simple and clockwise?");
    }

    // Returns true if p1,p2 and p3 form a clockwise triangle (returns false if anticlockwise, or all three on the same line)
    bool Clockwise(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector2 difference1 = (new Vector2(p2.x, p2.z) - new Vector2(p1.x, p1.z));
        Vector2 difference2 = (new Vector2(p3.x, p3.z) - new Vector2(p2.x, p2.z));
        // Take the dot product of the (normal of difference1) and (difference2):
        bool status = (-difference1.y * difference2.x + difference1.x * difference2.y) < 0;
        return (-difference1.y * difference2.x + difference1.x * difference2.y) < 0;
    }

    // Returns true if [testPoint] lies inside, or on the boundary, of the triangle given by the points p1,p2 and p3.
    bool InsideTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 testPoint)
    {
        if (Clockwise(p1, p2, p3))
            return !Clockwise(p2, p1, testPoint) && !Clockwise(p3, p2, testPoint) && !Clockwise(p1, p3, testPoint);
        else
            return !Clockwise(p1, p2, testPoint) && !Clockwise(p2, p3, testPoint) && !Clockwise(p3, p1, testPoint);
    }
}