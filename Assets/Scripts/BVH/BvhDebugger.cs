using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BvhDebugger : MonoBehaviour
{
    public GameObject MeshToDebug;

    private void Start()
    {
        
    }

    void OnDrawGizmos()
    {
        if (MeshToDebug)
        {
            Mesh mesh = MeshToDebug.GetComponent<MeshFilter>().sharedMesh;
            print(mesh.vertices.Length + "   " + mesh.GetIndices(0).Length);

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
            Gizmos.DrawWireMesh(mesh, MeshToDebug.transform.position, MeshToDebug.transform.rotation, MeshToDebug.transform.localScale);
        }
    }

    void DrawBvhNode(BVH bvh, Node node, int depth)
    {
        if (node.ChildIndex == 0)
        {
            Random.InitState(0);
            Gizmos.color = Random.ColorHSV();
            var bounds = node.Bounds;
            Gizmos.DrawWireCube(bounds.Center, bounds.Max - bounds.Min);
        }
        else
        {
            DrawBvhNode(bvh, bvh.AllNodes[node.ChildIndex + 0], depth + 1);
            DrawBvhNode(bvh, bvh.AllNodes[node.ChildIndex + 1], depth + 1);
        }
    }
}
