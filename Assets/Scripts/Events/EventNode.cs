using System;
using System.Collections.Generic;
using GameInventory.Items;
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
        
        [field: SerializeField]
        public bool IsRequiredItem { get; private set; }

        [SerializeField, ShowIf("IsRequiredItem")]
        private List<ItemData> _requiredItems;
        
        [field: SerializeField]
        public bool IsAddItem { get; private set; }

        [SerializeField, ShowIf("IsAddItem")]
        private List<ItemData> _addedItems;
        
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
            if (IsRequiredItem)
                panel.SpendItems(_requiredItems);

            if (IsAddItem)
                panel.AddItems(_addedItems);
                
            panel.UpdateText(text);
            panel.UpdateSprite(_image);
        }
    }
}