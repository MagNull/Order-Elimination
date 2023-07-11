using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Panels
{
    public class RewardItem : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TextMeshProUGUI _name;
        
        public void UpdateItemInfo(Sprite icon, string name)
        {
            _icon.sprite = icon ? icon : _icon.sprite;
            _name.text = name;
        }
    }
}