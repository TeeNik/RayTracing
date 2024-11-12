using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.speed = 0;
    }

    public void UpdateAnimator(float deltaTime)
    {
        _animator.speed = 1.0f;
        _animator.Update(deltaTime);
        _animator.speed = 0.0f;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            UpdateAnimator(Time.deltaTime);
        }
    }
}
