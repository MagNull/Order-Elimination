using OrderElimination;
using UnityEngine;

namespace RoguelikeMap.Point
{
    public class ShopPoint : OrderElimination.Point
    {
        public void Visit(Squad squad)
        {
            squad.VisitShopPoint(this, new DialogWindowFormat(WindowFormat.FullScreen, Vector3.zero, Vector3.one, "Shop Point"));
        }
    }
}