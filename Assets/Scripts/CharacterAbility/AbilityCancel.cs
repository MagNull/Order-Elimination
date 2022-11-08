using System;
using UnityEngine;

namespace CharacterAbility
{
    public class AbilityCancel : MonoBehaviour
    {
        [SerializeField]
        private AbilityButton[] _abilityButtons;


        public void ResetAbilityButtons()
        {
            foreach (var button in _abilityButtons)
            {
                button.ResetAbility();
            }    
        }
        
        private void OnEnable()
        {
            foreach(var button in _abilityButtons)
            {
                button.Clicked += OnAbilityButtonClicked;
            }
        }

        private void OnDisable()
        {
            foreach (var button in _abilityButtons)
            {
                button.Clicked -= OnAbilityButtonClicked;
            }
        }

        private void OnAbilityButtonClicked(AbilityButton abilityButton)
        {
            foreach(var button in _abilityButtons)
            {
                if(button != abilityButton)
                {
                    button.CancelAbilityCast();
                }
            }
        }
    }
}