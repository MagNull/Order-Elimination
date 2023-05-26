using CharacterAbility;
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
        
        public void InitializeInfo(AbilityInfo abilityInfo)
        {
            _icon.sprite = abilityInfo.Icon;
            _name.text = abilityInfo.Name;
            _description.text = abilityInfo.Description;
            _damage.text = "Не ебу где взять дамаг";
            _range.text = "И это тоже";
            _coolDown.text = $"{abilityInfo.CoolDown} x.";
        }
    }
}