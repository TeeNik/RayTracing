using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogEffect : MonoBehaviour
{
    public Color Color = Color.white;
    public Color Color2 = Color.white;
    [Range(0, .2f)] public float Density = 0.025f;
    [Range(0, 10)] public float Power = 6.0f;

    public Shader FogShader;
    
    [NonSerialized] private Material _fog;
    private static readonly int ColorID = Shader.PropertyToID("_FogColor");
    private static readonly int Color2ID = Shader.PropertyToID("_FogColor2");
    private static readonly int DensityID = Shader.PropertyToID("_FogDensity");
    private static readonly int PowerID = Shader.PropertyToID("_Power");

    public void ApplyFogToRenderTexture(RenderTexture source, RenderTexture destination)
    {
        if (_fog == null)
        {
            _fog = new Material(FogShader);
            _fog.hideFlags = HideFlags.HideAndDontSave;
        }
        
        _fog.SetColor(ColorID, Color);
        _fog.SetColor(Color2ID, Color2);
        _fog.SetFloat(DensityID, Density);
        _fog.SetFloat(PowerID, Power);
        
        Graphics.Blit(source, destination, _fog);
    }
}
