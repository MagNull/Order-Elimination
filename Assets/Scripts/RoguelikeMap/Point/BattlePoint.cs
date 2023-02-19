using System.Collections.Generic;
using OrderElimination;
using UnityEngine;

namespace RoguelikeMap.Point
{
    public class BattlePoint : OrderElimination.Point
    {
        private List<Character> _enemies;

        public IReadOnlyList<Character> Enemies;

        private void Start()
        {
            _enemies = new List<Character>();
        }

        public override void Visit(Squad squad)
        {
            squad.VisitBattlePoint(this, new DialogWindowData(WindowFormat.FullScreen, Vector3.zero, Vector3.one, PointInfo.Text));
        }
    }
}