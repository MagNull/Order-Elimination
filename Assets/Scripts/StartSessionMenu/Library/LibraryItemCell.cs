using Inventory;
using Inventory_Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ItemsLibrary
{
    public class LibraryItemCell : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;

        public void SetViewSettings(ItemView itemView)
        {
            _icon.sprite = itemView.Icon;
            _title.text = itemView.Name;
            _description.text = itemView.Description;
        }
    }
}