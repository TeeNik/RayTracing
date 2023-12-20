using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class RayTracingObject : MonoBehaviour
{
    private void OnEnable()
    {
        RayTracingManager.RegisterObject(this);
    }

    private void OnDisable()
    {
        RayTracingManager.UnregisterObject(this);
    }
}
