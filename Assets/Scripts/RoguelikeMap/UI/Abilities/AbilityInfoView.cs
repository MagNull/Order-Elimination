using CharacterAbility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap.UI.Abilities
{
    public class AbilityInfoView : MonoBehaviour
    {
        [SerializeField] 
        private Image _icon;
        [SerializeField]
        private TMP_Text _name;
        [SerializeField] 
        private TMP_Text _description;

        public void SetInfo(AbilityInfo abilityInfo)
        {
            _icon.sprite = abilityInfo.Icon;
            _name.text = abilityInfo.Name;
            _description.text = abilityInfo.Description;
        }
    }
}