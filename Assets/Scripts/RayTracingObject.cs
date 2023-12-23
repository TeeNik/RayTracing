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

    private void Update()
    {
        if (transform.hasChanged)
        {
            RayTracingManager.RegisterObject(this);
            RayTracingManager.UnregisterObject(this);
            transform.hasChanged = false;
        }
    }
}
