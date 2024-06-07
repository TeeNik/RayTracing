using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public BoundingBox Bounds = new();
    public List<BVHTriangle> Triangles = new();
    public Node ChildA;
    public Node ChildB;

    public Node() {}
    
    public Node(BoundingBox bounds, List<BVHTriangle> triangles)
    {
        Bounds = bounds;
        Triangles = triangles;
    }
    
}

public class BVH
{

    public Node root;
    public int MaxDepth = 1;
    
    public BVH(Vector3[] vertices, int[] indices)
    {
        BoundingBox bounds = new BoundingBox();

        foreach (var vertex in vertices)
        {
            bounds.GrowToInclude(vertex);
        }

        BVHTriangle[] triangles = new BVHTriangle[indices.Length / 3];
        for (int i = 0; i < indices.Length; i += 3)
        {
            Vector3 a = vertices[indices[i + 0]];
            Vector3 b = vertices[indices[i + 1]];
            Vector3 c = vertices[indices[i + 2]];
            triangles[i / 3] = new BVHTriangle(a, b, b);
        }

        root = new Node(bounds, new List<BVHTriangle>(triangles));
        Split(root);
    }

    public void Split(Node parent, int depth = 0)
    {
        if (depth == MaxDepth)
        {
            return;
        }

        parent.ChildA = new Node();
        parent.ChildB = new Node();

        foreach (var tri in parent.Triangles)
        {
            bool inA = tri.Center.x < parent.Bounds.Center.x;
            Node child = inA ? parent.ChildA : parent.ChildB;
            child.Triangles.Add(tri);
            child.Bounds.GrowToInclude(tri);
        }
        
        Split(parent.ChildA, depth + 1);
        Split(parent.ChildB, depth + 1);
        
    }
}
