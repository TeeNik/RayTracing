using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct RayTracingMaterial
{
    public Vector3 albedo;
    public Vector3 specular;
    public float smoothness;
    public Vector3 emission;
}

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class RayTracingObject : MonoBehaviour
{
    public RayTracingMaterial Material;

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
