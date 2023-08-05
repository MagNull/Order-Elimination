using System;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class BattlePointModel : FinalBattlePointModel
    {
        [Output] public PointModel exits;
        public override PointType Type => PointType.Battle;
    }
}