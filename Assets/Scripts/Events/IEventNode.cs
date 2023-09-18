using RoguelikeMap.UI.PointPanels;
using UnityEngine;
using XNode;

namespace OrderElimination.Events
{
    public abstract class IEventNode : Node
    {
        [SerializeField]
        protected string _name;
        
        public abstract void Process(EventPanel panel, int index = 0);
        
        public abstract void OnEnter(EventPanel panel);
        
        private void OnValidate() => name = _name == "" ? GetType().Name : _name;
    }
}