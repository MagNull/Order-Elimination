using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedCrosshair : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Animator _animator;
    [Header("Animation names mapping")]
    [SerializeField]
    private string _crosshairInStateName;

    private int _crosshairInState;

    private void Reset()
    {
        _animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        _crosshairInState = Animator.StringToHash(_crosshairInStateName);
    }

    public void PlayCrosshairInAnimation()
    {
        _animator.Play(_crosshairInState);
    }
}
