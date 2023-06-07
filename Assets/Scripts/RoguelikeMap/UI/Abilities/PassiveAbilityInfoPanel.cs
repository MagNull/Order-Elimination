using System.Collections.Generic;
using CharacterAbility;
using UnityEngine;

namespace RoguelikeMap.UI.Abilities
{
    public class PassiveAbilityInfoPanel : Panel
    {
        [SerializeField] 
        private List<AbilityInfoView> _views = new();

        public void InitializeInfo(AbilityInfo[] abilityInfos)
        {
            for (var i = 0; i < abilityInfos.Length; i++)
            {
                _views[i].SetInfo(abilityInfos[i]);   
            }
        }
    }
}