using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum MaterialFlag
{
    None,
    CheckerPattern = 1,
    HasTexture = 2,
    Invisible = 3,
}

[Serializable]
public struct RayTracingMaterial_Editor
{
    public Color albedo;
    public Color specular;
    public float smoothness;
    public Color emission;
    public float emissionStrength;
    public float specularChance;
    public MaterialFlag flag;

    public RayTracingMaterial GetMaterialForShader()
    {
        return new RayTracingMaterial()
        {
            albedo = new Vector3(albedo.r, albedo.g, albedo.b),
            specular = new Vector3(specular.r, specular.g, specular.b),
            smoothness = this.smoothness,
            emission = new Vector3(emission.r, emission.g, emission.b),
            emissionStrength = this.emissionStrength,
            specularChance = this.specularChance,
            flag = this.flag
        };
    }
}

public struct RayTracingMaterial
{
    public Vector3 albedo;
    public Vector3 specular;
    public float smoothness;
    public Vector3 emission;
    public float emissionStrength;
    public float specularChance;
    public MaterialFlag flag;
    
}

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class RayTracingObject : MonoBehaviour
{
    public RayTracingMaterial_Editor Material;

    [SerializeField, HideInInspector] int materialId;
    [SerializeField, HideInInspector] bool materialInited;

    private void OnValidate()
    {
        if (!EditorUtility.IsPersistent(this))
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                if (materialId != gameObject.GetInstanceID())
                {
                    renderer.sharedMaterial = new Material(renderer.sharedMaterial);
                    materialId = gameObject.GetInstanceID();
                }

                renderer.sharedMaterial.color = Material.albedo;
            }
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

    private void OnDrawGizmosSelected()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(renderer.bounds.center, renderer.bounds.extents);
    }
}
