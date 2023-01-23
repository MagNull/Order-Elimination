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

        public void AssignPassiveSkill(AbilityInfo passiveSkillInfo)
        {
            _icon.sprite = passiveSkillInfo.Icon;
            _name.text = passiveSkillInfo.Name;
            _description.text = passiveSkillInfo.Description;
        }
    } 
}
