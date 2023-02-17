using OrderElimination;

namespace RoguelikeMap.Point
{
    public class ShopPoint : OrderElimination.Point
    {
        public void Visit(Squad squad)
        {
            squad.VisitShopPoint();
        }
    }
}