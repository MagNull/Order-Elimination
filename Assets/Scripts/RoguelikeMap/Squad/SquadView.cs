using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
        public void OnMove(PlanetPoint planetPoint)
        {
            var target = planetPoint.transform.position +
                         new Vector3(-IconSize + (planetPoint.CountSquadOnPoint - 1) * 100f,
                             IconSize + 10f);
            var tween = _transform.DOMove(target, 0.5f);
            _transform.GetComponent<SpriteRenderer>().DOColor(Color.grey, DURATION);
            tween.OnComplete(() => onEndAnimation?.Invoke());
        }

        public void OnReadyMove()
        {
            _transform.GetComponent<SpriteRenderer>().DOColor(Color.white, DURATION);
        }
    }
}