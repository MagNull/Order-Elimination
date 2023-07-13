using RoguelikeMap.UI.PointPanels;
using UnityEngine;

namespace Events
{
    public class StartNode : EventNode
    {
        [SerializeField]
        private Sprite _sprite;
        
        public override void OnEnter(EventPanel panel)
        {
            panel.UpdateText(text);
            panel.UpdateSprite(_sprite);
        }
    }
}