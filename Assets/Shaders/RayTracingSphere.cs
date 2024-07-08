using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RayTracingSphere : MonoBehaviour
{
    public float Radius;
    public RayTracingMaterial_Editor Material;

    [SerializeField, HideInInspector] int materialId;
    [SerializeField, HideInInspector] bool materialInited;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (!EditorUtility.IsPersistent(this))
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

                renderer.sharedMaterial.color = Material.albedo;
            }
        }
#endif
    }
}
