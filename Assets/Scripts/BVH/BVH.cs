using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;



public class BVH
{
    public readonly List<Node> Nodes = new();
    public List<Triangle> Triangles; 

    public Node root;
    public int MaxDepth = 0;

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
    }

    public void Split(Node parent, int depth = 0)
    {
        if (depth == MaxDepth)
        {
            return;
        }

        parent.ChildIndex = Nodes.Count;
        Node childA = new Node() {TriangleIndex = parent.TriangleIndex};
        Node childB = new Node() {TriangleIndex = parent.TriangleIndex};
        Nodes.Add(childA);
        Nodes.Add(childB);

        Vector3 size = parent.Bounds.Size;
        Vector3 center = parent.Bounds.Center;

        int splitAxis = size.x > Mathf.Max(size.y, size.z) ? 0 : (size.y > size.z ? 1 : 2);
        float splitPos = splitAxis == 0 ? center.x : (splitAxis == 1 ? center.y : center.z);

        for (int i = parent.TriangleIndex; i < parent.TriangleIndex + parent.TriangleCount; ++i)
        {
            var tri = Triangles[i];
            float triCenter = splitAxis == 0 ? tri.Center.x : (splitAxis == 1 ? tri.Center.y : tri.Center.z);

            bool inA = triCenter < splitPos;
            Node child = inA ? childA : childB;
            child.TriangleCount++;
            //child.Bounds.GrowToInclude(tri);

            child.Bounds.GrowToInclude(tri.VertexA);
            child.Bounds.GrowToInclude(tri.VertexB);
            child.Bounds.GrowToInclude(tri.VertexC);

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