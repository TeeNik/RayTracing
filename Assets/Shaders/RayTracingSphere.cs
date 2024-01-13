using UnityEngine;

public class RayTracingSphere : MonoBehaviour
{
    public float Radius;
    public RayTracingMaterial Material;

    [SerializeField, HideInInspector] int materialId;
    [SerializeField, HideInInspector] bool materialInited;

    private void OnValidate()
    {
        float d = Radius * 2;
        transform.localScale = new Vector3(d, d, d);

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
}
