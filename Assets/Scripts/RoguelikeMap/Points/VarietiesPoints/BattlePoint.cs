using System.Collections.Generic;
using OrderElimination;
using RoguelikeMap.Panels;

namespace RoguelikeMap.Points.VarietiesPoints
{
    public class BattlePoint : Point
    {
        public override void Visit(Squad squad)
        {
            squad.VisitBattlePoint(this);
            if(PointView is null)
                InitializePointView();
        }

        public override void InitializePointView()
        {
            var panel = (BattlePanel)_panelGenerator.GetPanelByPointInfo(PointType.Battle);
            panel.UpdateEnemies(PointInfo.Enemies);
            PointView = new PointView(panel);
        }
    }
}