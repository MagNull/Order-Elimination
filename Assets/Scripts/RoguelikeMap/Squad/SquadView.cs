using System;
using DG.Tweening;
using RoguelikeMap.Panels;
using UnityEngine;
//using UnityEngine.Rendering.Universal;

namespace OrderElimination
{
    public class SquadView
    {
        public const float IconSize = 50f;
        public float DURATION = 0.5f;
        public float END_VALUE = 0.8f;
        
        private Transform _transform;
        private IPanel _panel;
        
        public event Action onEndAnimation;
        public SquadView(Transform transform)
        {
            _transform = transform;
        }

        public void SetPanel(IPanel panel)
        {
            _panel = panel;
        }

        public void SetActivePanel(bool isActive)
        {
            if (isActive)
                _panel.Open();
            else
                _panel.Close();
        }
        
        public void OnMove(Vector3 position)
        {
            var target = position +
                         new Vector3(-IconSize,
                             IconSize + 10f);
            var tween = _transform.DOMove(target, DURATION);
            tween.OnComplete(() => onEndAnimation?.Invoke());
        }
    }
}