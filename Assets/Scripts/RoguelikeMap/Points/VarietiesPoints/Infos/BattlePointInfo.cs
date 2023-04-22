using System;
using System.Collections.Generic;
using OrderElimination;
using UnityEngine;

namespace RoguelikeMap.Points.VarietiesPoints.Infos
{
    [Serializable]
    public class BattlePointInfo : VarietiesPoint
    {
        [SerializeField]
        private List<Character> _enemies;
        
        public override PointType PointType => PointType.Battle;
        public IReadOnlyList<IBattleCharacterInfo> Enemies => _enemies;
    }
}