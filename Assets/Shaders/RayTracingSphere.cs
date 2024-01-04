using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracingSphere : MonoBehaviour
{
    public float Radius;
    public RayTracingMaterial Material;

    private void OnValidate()
    {
        transform.localScale = new Vector3(Radius, Radius, Radius);
    }
}
