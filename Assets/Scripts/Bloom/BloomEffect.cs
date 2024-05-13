using System;
using UnityEngine;

public class BloomEffect : MonoBehaviour
{
    [Range(1, 16)] public int Iterations = 1;
    [Range(0, 10)] public float Threshold = 1;
    [Range(0, 1)] public float SoftThreshold = 0.5f;
    [Range(0, 10)] public float Intensity = 1;
    public Shader BloomShader;
    public bool Debug;
    
    [NonSerialized] private Material _bloom;
    private RenderTexture[] _textures = new RenderTexture[16];

    private const int BoxDownPrefilterPass = 0;
    private const int BoxDownPass = 1;
    private const int BoxUpPass = 2;
    private const int ApplyBloomPass = 3;
    private const int DebugBloomPass = 4;
    
    public void ApplyBloomToRenderTexture(RenderTexture source, RenderTexture destination)
    {
        if (_bloom == null)
        {
            _bloom = new Material(BloomShader);
            _bloom.hideFlags = HideFlags.HideAndDontSave;
        }
        
        float knee = Threshold * SoftThreshold;
        Vector4 filter;
        filter.x = Threshold;
        filter.y = filter.x - knee;
        filter.z = 2f * knee;
        filter.w = 0.25f / (knee + 0.00001f);
        _bloom.SetVector("_Filter", filter);
        _bloom.SetFloat("_Intensity", Mathf.GammaToLinearSpace(Intensity));
        
        int width = source.width / 2;
        int height = source.height / 2;
        RenderTextureFormat format = source.format;
        RenderTexture currentDest = _textures[0] = RenderTexture.GetTemporary(width, height, 0, format);
        Graphics.Blit(source, currentDest, _bloom, BoxDownPrefilterPass);

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
            Graphics.Blit(currentSource, currentDest, _bloom, BoxDownPass);
            currentSource = currentDest;
        }

        for (i -= 2; i >= 0; --i)
        {
            currentDest = _textures[i];
            _textures[i] = null;
            Graphics.Blit(currentSource, currentDest, _bloom, BoxUpPass);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDest;
        }

        if (Debug)
        {
            Graphics.Blit(currentSource, destination, _bloom, DebugBloomPass);
        }
        else
        {
            _bloom.SetTexture("_SourceTex", source);
            Graphics.Blit(currentSource, destination, _bloom, ApplyBloomPass);
        }
        RenderTexture.ReleaseTemporary(currentSource);
    }
}
