using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class ShaderUtils
{
    public static void CreateMaterial(ref Material material, Shader shader)
    {
        if (material == null || material.shader != shader)
        {
            material = new Material(shader);
        }
    }

    public static void CreateComputeBuffer<T>(ref ComputeBuffer buffer, List<T> data) where T : struct
    {
        int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));

        if (buffer != null)
        {
            if (data.Count == 0 || buffer.count != data.Count || buffer.stride != stride)
            {
                buffer.Release();
                buffer = null;
            }
        }

        if (data.Count != 0)
        {
            if (buffer == null)
            {
                buffer = new ComputeBuffer(data.Count, stride, ComputeBufferType.Structured);
            }
            buffer.SetData(data);
        }
    }

    public static void CreateRenderTexture(ref RenderTexture texture, int width, int height, FilterMode filterMode, GraphicsFormat format, string name, int depth, bool useMipMaps)
    {
        if (texture == null || !texture.IsCreated() || texture.width != width || texture.height != height || texture.graphicsFormat != format || texture.depth != depth || texture.useMipMap != useMipMaps)
        {
            if (texture != null)
            {
                texture.Release();
            }

            texture = new RenderTexture(width, height, depth);
            texture.graphicsFormat = format;
            texture.enableRandomWrite = true;
            texture.autoGenerateMips = false;
            texture.useMipMap = useMipMaps;
            texture.Create();
        }

        texture.name = name;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = filterMode;
    }
}
