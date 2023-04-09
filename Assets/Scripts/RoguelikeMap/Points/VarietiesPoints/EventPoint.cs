namespace RoguelikeMap.Points.VarietiesPoints
{
    public class EventPoint : Point
    {
        public override void InitializePointView()
        {
            var panel = _panelGenerator.GetPanelByPointInfo(PointType.Event);
            PointView = new PointView(panel);
        }
    }
}