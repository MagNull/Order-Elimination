using System.Collections.Generic;
using OrderElimination.AbilitySystem;
using UnityEngine;

namespace RoguelikeMap.UI.Abilities
{
    public class PassiveAbilityInfoPanel : Panel
    {
        [SerializeField] 
        private List<AbilityInfoView> _views = new();

        public void InitializeInfo(IPassiveAbilityData[] passiveAbilities)
        {
            foreach(var view in _views)
                view.SetActive(true);
            
            for (var i = 0; i < passiveAbilities.Length; i++)
            {
                var view = passiveAbilities[i].View;
                _views[i].SetInfo(view.Icon, view.Name, view.Description);   
            }
            
            for(var i = passiveAbilities.Length; i < _views.Count; i++)
                _views[i].SetActive(false);
        }
    }
}