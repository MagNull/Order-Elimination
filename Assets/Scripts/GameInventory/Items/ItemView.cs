using System;
using UnityEngine;

namespace GameInventory.Items
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
        public Sprite Icon => _icon != null ? _icon : Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.zero);
        public string Name => _name;
    }
}