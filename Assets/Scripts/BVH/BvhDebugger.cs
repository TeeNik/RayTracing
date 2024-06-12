using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BvhDebugger : MonoBehaviour
{
    public GameObject MeshToDebug;
    public GameObject RayGameObject;

    private void Start()
    {
        
    }

    void OnDrawGizmos()
    {
        if (MeshToDebug)
        {
            Mesh mesh = MeshToDebug.GetComponent<MeshFilter>().sharedMesh;

            var localToWorld = MeshToDebug.transform.localToWorldMatrix;
            var vertices = mesh.vertices;
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = (Vector3)(localToWorld * vertices[i]) + MeshToDebug.transform.position;
            }

            BVH bvh = new BVH(vertices, mesh.GetIndices(0));
            
            //Gizmos.color = Color.green;
            //var bounds = bvh.root.Bounds;
            //Gizmos.DrawWireCube(bounds.Center, bounds.Max - bounds.Min);
            
            DrawBvhNode(bvh, bvh.root, 0);
            
            Gizmos.color = Color.red;
            //Gizmos.DrawWireMesh(mesh, MeshToDebug.transform.position, MeshToDebug.transform.rotation, MeshToDebug.transform.localScale);

            if (RayGameObject)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(RayGameObject.transform.position, RayGameObject.transform.forward);
            }
        }
    }

    void DrawBvhNode(BVH bvh, Node node, int depth)
    {
        if (node.ChildIndex == 0)
        {
            var bounds = node.Bounds;
            Gizmos.DrawWireCube(bounds.Center, bounds.Max - bounds.Min);

            for (int i = node.TriangleIndex; i < node.TriangleIndex + node.TriangleCount; ++i)
            {
                var tri = bvh.Triangles[i];
                Gizmos.DrawLine(tri.VertexA, tri.VertexB);
                Gizmos.DrawLine(tri.VertexA, tri.VertexC);
                Gizmos.DrawLine(tri.VertexB, tri.VertexC);
            }
        }
        else
        {
            Gizmos.color = Color.yellow;
            DrawBvhNode(bvh, bvh.Nodes[node.ChildIndex + 0], depth + 1);
            Gizmos.color = Color.magenta;
            DrawBvhNode(bvh, bvh.Nodes[node.ChildIndex + 1], depth + 1);
        }
    }
}
