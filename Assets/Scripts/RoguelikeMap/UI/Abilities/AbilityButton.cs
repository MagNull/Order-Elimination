using System;
using CharacterAbility;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap.UI.Abilities
{
    public class AbilityButton : MonoBehaviour
    {
        [SerializeField]
        protected Image _abilityImage;
        
        private AbilityInfo _info;

        public event Action<AbilityInfo> OnClick;

        public virtual void SetAbilityInfo(AbilityInfo abilityInfo)
        {
            _info = abilityInfo;
            _abilityImage.sprite = abilityInfo.Icon;
        }
        
        public virtual void OnMouseDown()
        {
            OnClick?.Invoke(_info);
        }
    }
}