using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BVHTriangle
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
