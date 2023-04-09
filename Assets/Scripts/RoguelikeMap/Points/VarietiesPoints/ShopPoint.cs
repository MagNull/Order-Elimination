using RoguelikeMap.Panels;

namespace RoguelikeMap.Points.VarietiesPoints
{
    public class ShopPoint : Point
    {
        public override void InitializePointView()
        {
            var panel = (ShopPanel)_panelGenerator.GetPanelByPointInfo(PointType.Shop);
            PointView = new PointView(panel);
        }
    }
}