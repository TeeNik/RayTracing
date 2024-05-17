using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class FogRenderer : MonoBehaviour
{
    public FogEffect FogEffect;
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (FogEffect != null)
        {
            FogEffect.ApplyFogToRenderTexture(source, destination);
        }
    }
}
