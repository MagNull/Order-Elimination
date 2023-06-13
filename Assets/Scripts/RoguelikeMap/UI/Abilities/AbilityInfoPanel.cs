using CharacterAbility;
using OrderElimination.AbilitySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap.UI.Abilities
{
    public class AbilityInfoPanel : Panel
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TMP_Text _name;
        [SerializeField] 
        private TMP_Text _description;
        [SerializeField]
        private TMP_Text _damage;
        [SerializeField]
        private TMP_Text _range;
        [SerializeField] 
        private TMP_Text _coolDown;
        
        public void InitializeInfo(IActiveAbilityData activeAbility)
        {
            var view = activeAbility.View;
            _icon.sprite = view.Icon;
            _name.text = view.Name;
            _description.text = view.Description;
            _damage.text = "???";
            _range.text = "???";
            _coolDown.text = $"{activeAbility.GameRepresentation.CooldownTime} x.";
        }
    }
}