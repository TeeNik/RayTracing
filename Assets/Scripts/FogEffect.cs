using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class FogEffect : MonoBehaviour
{
    [Range(1, 16)] public int Iterations = 1;
    [Range(0, 10)] public float Threshold = 1;
    [Range(0, 1)] public float SoftThreshold = 0.5f;
    [Range(0, 10)] public float Intensity = 1;
    public Shader FogShader;
    public bool Debug;
    
    [NonSerialized] private Material _fog;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        ApplyFogToRenderTexture(source, destination);
    }
    
    public void ApplyFogToRenderTexture(RenderTexture source, RenderTexture destination)
    {
        if (_fog == null)
        {
            _fog = new Material(FogShader);
            _fog.hideFlags = HideFlags.HideAndDontSave;
        }
        
        Graphics.Blit(source, destination, _fog);
    }
}
