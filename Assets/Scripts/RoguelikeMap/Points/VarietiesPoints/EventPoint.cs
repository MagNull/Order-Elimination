using OrderElimination;

namespace RoguelikeMap.Points.VarietiesPoints
{
    public class EventPoint : Point
    {
        public override void Visit(Squad squad)
        {
            squad.VisitEventPoint(this);
            if(PointView is null)
                InitializePointView();
        }

        public override void InitializePointView()
        {
            var panel = _panelGenerator.GetPanelByPointInfo(PointType.Event);
            PointView = new PointView(panel);
        }
    }
}