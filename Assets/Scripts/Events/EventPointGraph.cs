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
        private EventPanel _panel;
        public IEventNode currentNode;
    
        public bool IsContainsBattle => CheckContainsBattle();

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
            // return StartNode.IsFork 
            //     ? CheckContainsBattle(StartNode.exits)
            //return CheckContainsBattle(StartNode.exits);
        }

        // private bool CheckContainsBattle(IEnumerable<EventInfo> eventInfos)
        // {
        //     return eventInfos.Select(x => x).Any(CheckContainsBattle);
        // }
        //
        // private bool CheckContainsBattle(EventInfo eventInfo)
        // {
        //     if (eventInfo.IsEnd)
        //         return false;
        //     if (eventInfo.IsFork)
        //         return CheckContainsBattle(eventInfo.exits);
        //         
        //     return eventInfo.exits is null ? eventInfo.IsBattle : CheckContainsBattle(eventInfo.exits);
        // }
    }
}