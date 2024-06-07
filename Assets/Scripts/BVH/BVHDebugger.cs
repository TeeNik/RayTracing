using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BVHDebugger : MonoBehaviour
{
    public GameObject MeshToDebug;
    private bool bInited = false;

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

            BVH bhv = new BVH(vertices, mesh.GetIndices(0));
            bInited = true;
            
            
            
            Gizmos.color = Color.green;
            var bounds = bhv.root.Bounds;
            Gizmos.DrawWireCube(bounds.Center, bounds.Max - bounds.Min);
            Gizmos.color = Color.red;
            Gizmos.DrawWireMesh(mesh, MeshToDebug.transform.position, MeshToDebug.transform.rotation, MeshToDebug.transform.localScale);
        }
    }
}
