using OrderElimination;
using UnityEngine;

namespace RoguelikeMap.Point
{
    public class ShopPoint : OrderElimination.Point
    {
        public override void Visit(Squad squad)
        {
            squad.VisitShopPoint(this, new DialogWindowData(WindowFormat.FullScreen, Vector3.zero, Vector3.one, PointInfo.Text));
        }
    }
}