using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingObject : MonoBehaviour, ITickable
{
    public Vector3 Offset = new Vector3(0, 0.2f, 0);
    public float Speed = 0.5f;

    private float _animationTime = 0.0f;
    private Vector3 _startPos;
    
    void Start()
    {
        _startPos = transform.position;
    }

    public void Tick(float deltaTime)
    {
        _animationTime += deltaTime;
        transform.position = _startPos + Offset * Mathf.Sin(_animationTime * Speed);
    }
}
