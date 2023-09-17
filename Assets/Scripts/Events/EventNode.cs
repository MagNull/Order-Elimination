using System;
using OrderElimination.Events;
using RoguelikeMap.UI.PointPanels;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using XNode;

namespace Events
{
    [Serializable]
    public class Empty { }
    
    [NodeWidth(300)]
    public class EventNode : IEventNode
    {
        [Input] public Empty entries;
        [Output] public Empty exits;

        [SerializeField]
        private Sprite _image;

        [SerializeField, MultiLineProperty, TextArea(10, 100)]
        protected string text;
        
        public override object GetValue(NodePort port)
        {
            return this;
        }

        public override void Process(EventPanel panel, int index = 0)
        {
            NodePort exitPort = GetOutputPort("exits");

            if (!exitPort.IsConnected) {
                Debug.LogWarning("Node isn't connected");
                return;
            }

            var node = exitPort.Connection.node as IEventNode;
            node.OnEnter(panel);
        }

        public override void OnEnter(EventPanel panel)
        {
            var eventGraph = graph as EventPointGraph;
            eventGraph.currentNode = this;
            panel.UpdateText(text);
            panel.UpdateSprite(_image);
        }
    }
}