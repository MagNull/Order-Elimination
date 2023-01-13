using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CharacterAbility;

namespace UIManagement.Elements
{
    public class PassiveSkillDescriptionCard : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;

        public void AssignPassiveSkill(AbilityView skillView)
        {
            _icon.sprite = skillView.AbilityIcon;
            _name.text = skillView.Name;
            _description.text = skillView.AbilityInfo.Description;
        }
    } 
}
