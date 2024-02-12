using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct RayTracingMaterial
{
    public enum MaterialFlag
    {
        None,
        CheckerPattern = 1,
        HasTexture = 2,
    }

    public Vector3 albedo;
    public Vector3 specular;
    public float smoothness;
    public Vector3 emission;
    public float specularChance;
    public MaterialFlag flag;
}

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class RayTracingObject : MonoBehaviour
{
    public RayTracingMaterial Material;

    [SerializeField, HideInInspector] int materialId;
    [SerializeField, HideInInspector] bool materialInited;

    private void OnValidate()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            if (materialId != gameObject.GetInstanceID())
            {
                renderer.sharedMaterial = new Material(renderer.sharedMaterial);
                materialId = gameObject.GetInstanceID();
            }

            Vector3 color = Material.albedo;
            renderer.sharedMaterial.color = new Color(color.x, color.y, color.z);
        }
    }

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
