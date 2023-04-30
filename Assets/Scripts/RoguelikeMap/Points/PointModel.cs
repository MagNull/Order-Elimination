using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeMap.Points
{
    [Serializable]
    public class PointModel
    {
        public IReadOnlyList<int> NextPoints => _nextPointsIndex;
        public virtual PointType Type => PointType.None;
        public int Index;
        
        [SerializeField]
        private List<int> _nextPointsIndex;
    }
}