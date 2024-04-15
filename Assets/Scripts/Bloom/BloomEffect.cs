using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class BloomEffect : MonoBehaviour
{
    [Range(1, 16)] public int Iterations = 1;

    private RenderTexture[] _textures = new RenderTexture[16];
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int width = source.width / 2;
        int height = source.height / 2;
        RenderTextureFormat format = source.format;
        RenderTexture currentDest = _textures[0] = RenderTexture.GetTemporary(width, height, 0, format);
        Graphics.Blit(source, currentDest);

        RenderTexture currentSource = currentDest;

        int i = 1;
        for (; i < Iterations; ++i)
        {
            width /= 2;
            height /= 2;
            if (height < 2)
            {
                break;
            }
            currentDest = _textures[i] = RenderTexture.GetTemporary(width, height, 0, format);
            Graphics.Blit(currentSource, currentDest);
            currentSource = currentDest;
        }

        for (i -= 2; i >= 0; --i)
        {
            currentDest = _textures[i];
            _textures[i] = null;
            Graphics.Blit(currentSource, currentDest);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDest;
        }
        
        
        Graphics.Blit(currentSource, destination);
        RenderTexture.ReleaseTemporary(currentSource);
    }
}
