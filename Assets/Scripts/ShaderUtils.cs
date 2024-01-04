using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShaderUtils
{
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
                buffer = new ComputeBuffer(data.Count, stride);
            }
            buffer.SetData(data);
        }
    }
}
