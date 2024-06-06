using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public BoundingBox Bounds = new();
    public List<BVHTriangle> Triangles = new();
    public Node ChildA;
    public Node ChildB;

    public Node(BoundingBox bounds, List<BVHTriangle> triangles)
    {
        Bounds = bounds;
        Triangles = triangles;
    }
    
}

public class BVH : MonoBehaviour
{

    public Node root;
    
    public BVH(Vector3[] vertices, int[] indices)
    {
        BoundingBox bounds = new BoundingBox();

        foreach (var vertex in vertices)
        {
            bounds.GrowToInclude(vertex);
        }

        BVHTriangle[] triangles = new BVHTriangle[indices.Length / 3];
        for (int i = 0; i < indices.Length; ++i)
        {
            Vector3 a = vertices[indices[i + 0]];
            Vector3 b = vertices[indices[i + 1]];
            Vector3 c = vertices[indices[i + 2]];
            triangles[i] = new BVHTriangle(a, b, b);
        }

        root = new Node(bounds, new List<BVHTriangle>(triangles));
    }
}
