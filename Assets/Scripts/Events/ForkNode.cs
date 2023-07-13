using System.Collections.Generic;
using System.Linq;
using OrderElimination.Events;
using RoguelikeMap.UI.PointPanels;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace Events
{
    public class ForkNode : Node, IEventNode
    {
        [Input] public Empty entries;
        [Output] public Empty exits;
        
        [field: SerializeField]
        public bool IsRandom { get; private set; }
        
        [TabGroup("Answers")]
        [SerializeReference, HideIf("IsRandom")]
        private List<string> _answers;

        private bool _isSelect = true;
        
        public void Process(EventPanel panel, int index = 0)
        {
            if (IsRandom)
                index = Random.Range(0, Outputs.First().ConnectionCount);
            var ports = GetPort("exits").GetConnections();
            if(index > ports.Count)
                Debug.LogError("Invalid port index");
            var node = ports[index].node as IEventNode;
            node.OnEnter(panel);
        }

        public void OnEnter(EventPanel panel)
        {
            var eventGraph = graph as EventPointGraph;
            eventGraph.currentNode = this;
            if (IsRandom)
            {
                Process(panel);
                return;
            }
            panel.UpdateAnswersText(_answers);
        }
    }
}