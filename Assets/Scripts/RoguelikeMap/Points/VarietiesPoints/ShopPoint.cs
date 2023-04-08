using OrderElimination;

namespace RoguelikeMap.Points
{
    public class ShopPoint : Points.Point
    {
        public override void Visit(Squad squad)
        {
            squad.VisitShopPoint(this);
        }
    }
}