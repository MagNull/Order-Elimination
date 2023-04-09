namespace RoguelikeMap.Points.VarietiesPoints
{
    public class SafeZonePoint : Point
    {
        public override void InitializePointView()
        {
            var panel = _panelGenerator.GetPanelByPointInfo(PointType.SafeZone);
            PointView = new PointView(panel);
        }
    }
}