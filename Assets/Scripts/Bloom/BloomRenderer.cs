using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class BloomRenderer : MonoBehaviour
{
    public BloomEffect BloomEffect;
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (BloomEffect != null)
        {
            BloomEffect.ApplyBloomToRenderTexture(source, destination);
        }
    }
}
