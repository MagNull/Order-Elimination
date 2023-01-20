using System;
using UnityEngine;

namespace OrderElimination.BM
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnvironmentObjectView : MonoBehaviour, IBattleObjectView
    {
        private SpriteRenderer _spriteRenderer;

        private EnvironmentObject _environmentObject;
        public event Action<IBattleObjectView> Disabled;
        public GameObject GameObject => gameObject;
        public IBattleObject Model => _environmentObject;
        
        public void Init(EnvironmentObject environmentObject, Sprite image)
        {
            _environmentObject = environmentObject;
            _environmentObject.Destroyed += OnDestroyed;
            BattleSimulation.RoundStarted += OnRoundStart;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = image;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void SetImage(Sprite image)
        {
            _spriteRenderer.sprite = image;
        }

        public void OnDamaged(TakeDamageInfo damageInfo)
        {
            
        }

        private void OnDestroyed()
        {
            Disable();
        }

        private void OnDisable()
        {
            BattleSimulation.RoundStarted -= OnRoundStart;
        }

        private void OnRoundStart()
        {
            _environmentObject.OnRoundStart();
        }
    }
}