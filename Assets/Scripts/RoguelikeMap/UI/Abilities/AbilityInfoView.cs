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

        public void SetInfo(Sprite icon, string name, string description)
        {
            _icon.sprite = icon;
            _name.text = name;
            _description.text = description;
        }
    }
}