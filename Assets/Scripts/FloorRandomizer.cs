using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorRandomizer : MonoBehaviour
{
    public float MinY = 0.5f;
    public float MaxY = 1.5f;

    [ContextMenu("Randomize Scale")]
    public void RandomizeScale()
    {
        foreach (Transform child in transform)
        {
            float y = Random.Range(MinY, MaxY);
            child.transform.localScale = new Vector3(child.transform.localScale.x, y, child.transform.localScale.z);
        }
    }
}
