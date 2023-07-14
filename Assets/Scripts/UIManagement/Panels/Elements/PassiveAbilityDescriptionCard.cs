using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UIManagement.Elements
{
    public class PassiveAbilityDescriptionCard : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;

        public void UpdateAbilityData(string name, Sprite icon, string description)
        {
            _icon.sprite = icon;
            _name.text = name;
            _description.text = description;
        }
    } 
}
