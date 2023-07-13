using System;
using System.Linq;
using OrderElimination.Events;
using RoguelikeMap.UI.PointPanels;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace Events
{
    [Serializable]
    public class Empty { }
    
    public class EventNode : Node, IEventNode
    {
        [Input] public Empty entries;
        [Output] public Empty exits;

        [SerializeField]
        protected string text;
        
        public override object GetValue(NodePort port)
        {
            return this;
        }

        public virtual void Process(EventPanel panel, int index = 0)
        {
            NodePort exitPort = GetOutputPort("exits");

            if (!exitPort.IsConnected) {
                Debug.LogWarning("Node isn't connected");
                return;
            }

            var node = exitPort.Connection.node as IEventNode;
            node.OnEnter(panel);
        }

        public virtual void OnEnter(EventPanel panel)
        {
            var eventGraph = graph as EventPointGraph;
            eventGraph.currentNode = this;
            panel.UpdateText(text);
        }
    }
}