using OrderElimination.Events;
using RoguelikeMap.UI.PointPanels;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace Events
{
    public class StartNode : Node, IEventNode
    {
        [Output] public Empty exits;
        
        [SerializeField, MultiLineProperty, TextArea(10, 100)]
        private string _text;
        
        [SerializeField]
        private Sprite _sprite;

        public void Process(EventPanel panel, int index = 0)
        {
            NodePort exitPort = GetOutputPort("exits");

            if (!exitPort.IsConnected) {
                Debug.LogWarning("Node isn't connected");
                return;
            }

            var node = exitPort.Connection.node as IEventNode;
            node.OnEnter(panel);
        }

        public void OnEnter(EventPanel panel)
        {
            panel.UpdateText(_text);
            panel.UpdateSprite(_sprite);
        }
    }
}