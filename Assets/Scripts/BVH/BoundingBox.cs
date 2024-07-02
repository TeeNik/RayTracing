using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Triangle
{
    public Vector3 VertexA;
    public Vector3 VertexB;
    public Vector3 VertexC;

    public Vector3 Min;
    public Vector3 Max;
    public Vector3 Center;
        
    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        VertexA = a;
        VertexB = b;
        VertexC = c;
        
        Min = Vector3.Min(Vector3.Min(VertexA, VertexB), VertexC);
        Max = Vector3.Max(Vector3.Max(VertexA, VertexB), VertexC);
        Center = (VertexA + VertexB + VertexC) / 3;
    }
}

public struct TriangleData
{
    public Vector3 VertexA;
    public Vector3 VertexB;
    public Vector3 VertexC;

    public TriangleData(Triangle Tri)
    {
        VertexA = Tri.VertexA;
        VertexB = Tri.VertexB;
        VertexC = Tri.VertexC;
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

    public void GrowToInclude(Triangle triangle)
    {
        Min.x = triangle.Min.x < Min.x ? triangle.Min.x : Min.x;
        Min.y = triangle.Min.y < Min.y ? triangle.Min.y : Min.y;
        Min.z = triangle.Min.z < Min.z ? triangle.Min.z : Min.z;
        
        Max.x = triangle.Max.x > Max.x ? triangle.Max.x : Max.x;
        Max.y = triangle.Max.y > Max.y ? triangle.Max.y : Max.y;
        Max.z = triangle.Max.z > Max.z ? triangle.Max.z : Max.z;
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
    public Vector3 BoundsMin;
    public Vector3 BoundsMax;
    public int TriangleIndex;
    public int TriangleCount;
    public int ChildIndex;

    public NodeData(Node node)
    {
        BoundsMin = node.Bounds.Min;
        BoundsMax = node.Bounds.Max;
        TriangleIndex = node.TriangleIndex;
        TriangleCount = node.TriangleCount;
        ChildIndex = node.ChildIndex;
    }
}
