using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BVHTriangle
{
    public Vector3 VertexA;
    public Vector3 VertexB;
    public Vector3 VertexC;

    public Vector3 Center => (VertexA + VertexB + VertexC) / 3;

    public BVHTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        VertexA = a;
        VertexB = b;
        VertexC = c;
    }
}

public struct BoundingBoxData
{
    public Vector3 Min;
    public Vector3 Max;

    public BoundingBoxData(BoundingBox box)
    {
        Min = box.Min;
        Max = box.Max;
    }
}

public class BoundingBox
{
    public Vector3 Min = Vector3.positiveInfinity;
    public Vector3 Max = Vector3.negativeInfinity;
    public Vector3 Center => (Min + Max) * 0.5f;
    public Vector3 Size => Max - Min;

    public void GrowToInclude(Vector3 point)
    {
        Min = Vector3.Min(Min, point);
        Max = Vector3.Max(Max, point);
    }

    public void GrowToInclude(BVHTriangle triangle)
    {
        GrowToInclude(triangle.VertexA);
        GrowToInclude(triangle.VertexB);
        GrowToInclude(triangle.VertexC);
    }
}

public class Node
{
    public BoundingBox Bounds = new();
    public int TriangleIndex;
    public int TriangleCount;
    public int ChildIndex = 0; //second child is always +1

    public Node()
    {
    }

    public Node(BoundingBox bounds)
    {
        Bounds = bounds;
    }
}

public struct NodeData
{
    public BoundingBox Bounds;
    public int TriangleIndex;
    public int TriangleCount;
    public int ChildIndex;

    public NodeData(Node node)
    {
        Bounds = node.Bounds;
        TriangleIndex = node.TriangleIndex;
        TriangleCount = node.TriangleCount;
        ChildIndex = node.ChildIndex;
    }
}
