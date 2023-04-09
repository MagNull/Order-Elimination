using System;
using DG.Tweening;
using UnityEngine;
//using UnityEngine.Rendering.Universal;

namespace OrderElimination
{
    public class SquadView
    {
        public const float IconSize = 50f;
        public float DURATION = 0.6f;
        public float END_VALUE = 0.8f;
        private Transform _transform;
        public event Action onEndAnimation; 
        public SquadView(Transform transform)
        {
            _transform = transform;
        }

        //TODO(Иван): Magic numbers
        public void OnMove(Vector3 position)
        {
            var target = position +
                         new Vector3(-IconSize,
                             IconSize + 10f);
            var tween = _transform.DOMove(target, 0.5f);
            tween.OnComplete(() => onEndAnimation?.Invoke());
        }
    }
}