using System;
using UnityEngine;

namespace CharacterAbility
{
    //TODO(САНО): Refactor
    public class AbilityPanel : MonoBehaviour
    {
        [SerializeField]
        private AbilityButton[] _abilityButtons;


        public void ResetAbilityButtons()
        {
            foreach (var button in _abilityButtons)
            {
                button.RemoveAbility();
            }    
        }
        
        private void OnEnable()
        {
            foreach(var button in _abilityButtons)
            {
                button.Clicked += OnAbilityButtonClicked;
                button.Casted += OnAbilityCasted;
            }
        }

        private void OnDisable()
        {
            foreach (var button in _abilityButtons)
            {
                button.Clicked -= OnAbilityButtonClicked;
                button.Casted -= OnAbilityCasted;
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

        private void OnAbilityCasted()
        {
            foreach(var button in _abilityButtons)
            {
                button.CheckUsePossibility();
            }
        }
    }
}