using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingObject : MonoBehaviour, ITickable
{
    public Vector3 Offset = new Vector3(0, 0.2f, 0);
    public Vector3 RotationOffset;
    public float Speed = 0.5f;

    private float _animationTime = 0.0f;
    private Vector3 _startPos;
    private Quaternion _startRot;
    
    void Awake()
    {
        _startPos = transform.position;
        _startRot = transform.rotation;
    }

    public void Tick(float deltaTime, bool debug = false)
    {
        _animationTime += deltaTime;
        transform.position = _startPos + Offset * Mathf.Sin(_animationTime * Speed);
        transform.rotation = _startRot * Quaternion.Euler(RotationOffset * Mathf.Sin(Mathf.PI * _animationTime * Speed));

        if (debug)
        {
            print("BouncingObject: " + gameObject.name + "  rot:  " + transform.rotation.eulerAngles + "  animTime: " + _animationTime);
        }
    }
}
