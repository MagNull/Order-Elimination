using System;
using DG.Tweening;
using UnityEngine;

namespace OrderElimination
{
    public class SquadView
    {
        private Transform _transform;
        public event Action onEndAnimation; 
        public SquadView(Transform transform)
        {
            _transform = transform;
        }

        //TODO(Иван): Magic numbers
        public void OnMove(PlanetPoint planetPoint)
        {
            var target = planetPoint.transform.position +
                         new Vector3(-StrategyMap.IconSize + (planetPoint.CountSquadOnPoint - 1) * 100f,
                             StrategyMap.IconSize + 10f);
            var tween = _transform.DOMove(target, 0.5f);
            tween.OnComplete(() => onEndAnimation?.Invoke());
        }
    }
}