using OrderElimination;

namespace RoguelikeMap.Point
{
    public class BattlePoint : OrderElimination.Point
    {
        public void Visit(Squad squad)
        {
            squad.VisitBattlePoint();
        }
    }
}