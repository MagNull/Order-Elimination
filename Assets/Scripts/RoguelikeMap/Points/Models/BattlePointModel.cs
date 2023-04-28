using System;
using System.Collections.Generic;
using OrderElimination;
using UnityEngine;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class BattlePointModel : PointModel
    {
        [SerializeField]
        private List<Character> _enemies;
        [SerializeField]
        private int _mapNumber;
        
        public override PointType Type => PointType.Battle;
        public IReadOnlyList<IBattleCharacterInfo> Enemies => _enemies;
        public int MapNumber => _mapNumber;
    }
}