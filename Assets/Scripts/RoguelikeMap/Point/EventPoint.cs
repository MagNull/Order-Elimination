using OrderElimination;

namespace RoguelikeMap.Point
{
    public class EventPoint : OrderElimination.Point
    {
        public string Text { get; set; }
        public void Visit(Squad squad)
        {
            squad.VisitEventPoint(this);
        }
    }
}