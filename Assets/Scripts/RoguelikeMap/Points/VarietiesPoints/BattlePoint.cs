using System.Collections.Generic;
using System.Linq;
using RoguelikeMap.Panels;

namespace RoguelikeMap.Points.VarietiesPoints
{
    public class BattlePoint : Point
    {
        public new BattlePointInfo PointInfo => (BattlePointInfo)base.PointInfo;
        public IReadOnlyList<IBattleCharacterInfo> Enemies => PointInfo.Enemies;


        public override void InitializePointView()
        {
            var panel = (BattlePanel)_panelGenerator.GetPanelByPointInfo(PointType.Battle);
            panel.UpdateEnemies(PointInfo.Enemies);
            PointView = new PointView(panel);
        }
    }
}