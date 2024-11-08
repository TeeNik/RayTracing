using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingObject : MonoBehaviour
{
    public Vector3 Offset = new Vector3(0, 0.2f, 0);
    public float Speed = 0.5f;
    
    private Vector3 _startPos;
    void Start()
    {
        _startPos = transform.position;
    }

    void Update()
    {
        transform.position = _startPos + Offset * Mathf.Sin(Time.time * Speed);
    }
}
