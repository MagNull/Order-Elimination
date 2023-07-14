using RoguelikeMap.UI.PointPanels;

namespace OrderElimination.Events
{
    public interface IEventNode
    {
        public void Process(EventPanel panel, int index = 0);
        
        public void OnEnter(EventPanel panel);
    }
}