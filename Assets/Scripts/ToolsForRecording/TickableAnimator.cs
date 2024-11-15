using UnityEngine;

public class TickableAnimator : MonoBehaviour, ITickable
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

    public void Tick(float deltaTime)
    {
        UpdateAnimator(deltaTime);
    }
}
