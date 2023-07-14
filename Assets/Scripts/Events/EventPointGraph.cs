using OrderElimination.Events;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;
using XNode;

namespace Events
{
    [CreateAssetMenu]
    public class EventPointGraph : NodeGraph
    {
        [SerializeField] 
        private StartNode _startNode;
        [field: SerializeField]
        public bool IsContainsBattle { get; private set; }
        
        private EventPanel _panel;
        public IEventNode currentNode;
        
        public void Process(EventPanel panel)
        {
            if (_panel is not null)
                _panel.OnAnswerClick -= NextNode;
            _panel = panel;
            currentNode = _startNode;
            _startNode.OnEnter(panel);
            _panel.OnAnswerClick += NextNode;
        }

        private void NextNode(int index)
        {
            currentNode.Process(_panel, index);
        }

        private bool CheckContainsBattle()
        {
            return false;
        }
    }
}