namespace RoguelikeMap.Points.VarietiesPoints
{
    public class EventPoint : Point
    {
        protected override void InitializePointView()
        {
            var panel = _panelGenerator.GetPanelByPointInfo(PointType.Event);
            PointView = new PointView(panel);
        }
    }
}