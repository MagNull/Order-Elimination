using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace ItemsLibrary
{

    public class LibraryItemView : MonoBehaviour
    {
        [SerializeField] private Image _itemSprite;
        [SerializeField] private TextMeshProUGUI _itemName;
        [SerializeField] private TextMeshProUGUI _itemDescription;

        private bool _isSeen = false;
        private LibraryItemInfo _info;
        public void SetItemInfo(LibraryItemInfo info)
        {
        
        }

        public void ChangeInfoVisibility(bool status)
        {
            //прятать или показывать информацию об объекте
        }
    }    
}