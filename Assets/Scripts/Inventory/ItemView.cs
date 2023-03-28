using System;
using UnityEngine;

namespace Inventory
{
    [Serializable]
    public class ItemView
    {
        [SerializeField]
        private string _name;
        [SerializeField][TextArea(1, 10)]
        private string _description;
        [SerializeField]
        private Sprite _icon;
        
        public string Description => _description;
        public Sprite Icon => _icon;
        public string Name => _name;
    }
}