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
    public int MaxDepth = 3;
    
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

        Vector3 size = parent.Bounds.Size;
        Vector3 center = parent.Bounds.Center;
        
        int splitAxis = size.x > Mathf.Max(size.y, size.z) ? 0 : (size.y > size.z ? 1 : 2);
        float splitPos = splitAxis == 0 ? center.x : (splitAxis == 1 ? center.y : center.z);

        foreach (var tri in parent.Triangles)
        {
            float triCenter = splitAxis == 0 ? tri.Center.x : (splitAxis == 1 ? tri.Center.y : tri.Center.z);

            bool inA = triCenter < splitPos;
            Node child = inA ? parent.ChildA : parent.ChildB;
            child.Triangles.Add(tri);
            //child.Bounds.GrowToInclude(tri);
            
            child.Bounds.GrowToInclude(tri.VertexA);
            child.Bounds.GrowToInclude(tri.VertexB);
            child.Bounds.GrowToInclude(tri.VertexC);
        }
        
        Split(parent.ChildA, depth + 1);
        Split(parent.ChildB, depth + 1);
        
    }
}
