using OrderElimination;
using RoguelikeMap.Points.VarietiesPoints.Infos;
using RoguelikeMap.SquadInfo;

namespace RoguelikeMap.Points.VarietiesPoints
{
    public class EventPoint : Point
    {
        public new EventPointInfo PointInfo => (EventPointInfo)base.PointInfo;
        
        protected override void InitializePointView()
        {
            var panel = _panelGenerator.GetPanelByPointInfo(PointType.Event);
            PointView = new PointView(panel);
        }

        public override void Visit(Squad squad)
        {
            base.Visit(squad);
            PointView.SetPointInfo(PointInfo);
        }
    }
}