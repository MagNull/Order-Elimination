using OrderElimination;
using RoguelikeMap.Panels;

namespace RoguelikeMap.Points.VarietiesPoints
{
    public class ShopPoint : Point
    {
        public override void Visit(Squad squad)
        {
            squad.VisitShopPoint(this);
            if(PointView is null)
                InitializePointView();
        }

        public override void InitializePointView()
        {
            var panel = (ShopPanel)_panelGenerator.GetPanelByPointInfo(PointType.Shop);
            PointView = new PointView(panel);
        }
    }
}