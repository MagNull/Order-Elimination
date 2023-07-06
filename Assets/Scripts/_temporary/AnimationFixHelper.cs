using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class AnimationFixHelper : MonoBehaviour
{
    private AnimationSceneContext _animationSceneContext;

    [Inject]
    private void Construct(IBattleContext battleContext)
    {
        _animationSceneContext = battleContext.AnimationSceneContext;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            _animationSceneContext.StopAllPlayingAnimations();
        }
    }
}
