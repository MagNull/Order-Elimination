using OrderElimination;

namespace RoguelikeMap.Point
{
    public class EventPoint : OrderElimination.Point
    {
        public void Visit(Squad squad)
        {
            squad.VisitEventPoint();
        }
    }
}