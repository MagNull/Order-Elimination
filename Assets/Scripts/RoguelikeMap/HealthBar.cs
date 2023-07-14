using System;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;

        private GameCharacter _gameCharacter;
           
        public void SetGameCharacter(GameCharacter character)
        {
            _gameCharacter = character ?? throw new NullReferenceException("GameCharacter is null");
            
            OnHealthChanged(character);
            character.StatsChanged += OnHealthChanged;
        }

        private void OnEnable()
        {
            if (_gameCharacter != null)
                _gameCharacter.StatsChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            if (_gameCharacter != null)
                _gameCharacter.StatsChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(GameCharacter character)
        {
            _slider.maxValue = character.CharacterStats.MaxHealth;
            _slider.value = character.CurrentHealth;
        }

        [Button]
        public void Damage()
        {
            _gameCharacter.CurrentHealth -= 10;
        }
    }
}