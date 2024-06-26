using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;



public class BVH
{
    public readonly List<Node> Nodes = new();
    public List<Triangle> Triangles; 

    public Node root;
    public int MaxDepth = 20;

    public readonly List<NodeData> NodesForBuffer;
    public readonly List<TriangleData> TrianglesForBuffer;

    public BVH(Vector3[] vertices, int[] indices)
    {
        BoundingBox bounds = new BoundingBox();

        foreach (var vertex in vertices)
        {
            bounds.GrowToInclude(vertex);
        }

        Triangles = new List<Triangle>();
        for (int i = 0; i < indices.Length; i += 3)
        {
            Vector3 a = vertices[indices[i + 0]];
            Vector3 b = vertices[indices[i + 1]];
            Vector3 c = vertices[indices[i + 2]];
            Triangles.Add(new Triangle(a, b, c));
        }

        root = new Node(bounds);
        //TODO refactor
        root.TriangleIndex = 0;
        root.TriangleCount = Triangles.Count;
        
        Nodes.Add(root);
        Split(root);

        NodesForBuffer = Nodes.Select(n => new NodeData(n)).ToList();
        TrianglesForBuffer = Triangles.Select(t => new TriangleData(t)).ToList();
        
        Nodes.Clear();
        Triangles.Clear();
    }

    public float NodeCost(Vector3 size, int numOfTriangles)
    {
        float halfSurface = size.x * size.y + size.x * size.z + size.y * size.z;
        return halfSurface * numOfTriangles;
    }

    void ChooseSplit(Node node, out int axis, out float pos, out float cost)
    {
        const int TestsPerAxis = 5;
        cost = float.MaxValue;
        pos = 0.0f;
        axis = 0;

        for (int testAxis = 0; testAxis < 3; ++testAxis)
        {
            float boundStart = node.Bounds.Min[testAxis];
            float boundEnd = node.Bounds.Max[testAxis];

            for (int i = 0; i < TestsPerAxis; ++i)
            {
                float split = (float)(i + 1) / (TestsPerAxis + 1);
                float testPos = boundStart + (boundEnd - boundStart) * split;
                float testCost = EValuateSplit(node, axis, testPos);

                if (testCost < cost)
                {
                    cost = testCost;
                    pos = testPos;
                    axis = testAxis;
                }
            }
        }
    }

    float EValuateSplit(Node node, int axis, float pos)
    {
        BoundingBox boundsA = new BoundingBox();
        BoundingBox boundsB = new BoundingBox();
        int triesInA = 0;
        int triesInB = 0;

        for (int i = node.TriangleIndex; i < node.TriangleIndex + node.TriangleCount; ++i)
        {
            Triangle tri = Triangles[i];
            if (tri.Center[axis] < pos)
            {
                boundsA.GrowToInclude(tri);
                ++triesInA;
            }
            else
            {
                boundsB.GrowToInclude(tri);
                ++triesInB;
            }
        }
        
        return NodeCost(boundsA.Size, triesInA) + NodeCost(boundsB.Size, triesInB);
    }

    public void Split(Node parent, int depth = 0)
    {
        if (depth == MaxDepth)
        {
            return;
        }

        int splitAxis;
        float splitPos, cost;
        ChooseSplit(parent, out splitAxis, out splitPos, out cost);
        if (cost >= NodeCost(parent.Bounds.Size, parent.TriangleCount))
        {
            return;
        }

        parent.ChildIndex = Nodes.Count;
        Node childA = new Node() {TriangleIndex = parent.TriangleIndex};
        Node childB = new Node() {TriangleIndex = parent.TriangleIndex};
        Nodes.Add(childA);
        Nodes.Add(childB);

        for (int i = parent.TriangleIndex; i < parent.TriangleIndex + parent.TriangleCount; ++i)
        {
            bool inA = Triangles[i].Center[splitAxis] < splitPos;
            Node child = inA ? childA : childB;
            child.Bounds.GrowToInclude(Triangles[i]);
            child.TriangleCount++;

            if (inA)
            {
                int swap = child.TriangleIndex + child.TriangleCount - 1;
                (Triangles[i], Triangles[swap]) = (Triangles[swap], Triangles[i]);
                childB.TriangleIndex++;
            }
        }

        Split(childA, depth + 1);
        Split(childB, depth + 1);
    }
}