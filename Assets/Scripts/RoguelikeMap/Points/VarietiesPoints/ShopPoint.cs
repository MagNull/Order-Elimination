using RoguelikeMap.Panels;

namespace RoguelikeMap.Points.VarietiesPoints
{
    public class ShopPoint : Point
    {
        protected override void InitializePointView()
        {
            var panel = (ShopPanel)_panelGenerator.GetPanelByPointInfo(PointType.Shop);
            PointView = new PointView(panel);
        }
    }
}