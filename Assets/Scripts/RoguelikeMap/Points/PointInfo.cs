using System.Collections.Generic;
using RoguelikeMap.Points.VarietiesPoints.Infos;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoguelikeMap.Points
{
    [CreateAssetMenu(fileName = "PointInfo", menuName = "Point/New PointInfo")]
    public class PointInfo : SerializedScriptableObject
    {
        [SerializeField]
        private Vector3 _position;
        [SerializeField]
        private List<PointInfo> _nextPoints;
        [SerializeReference]
        private VarietiesPoint _varietiesPoint;
        
        public Vector3 Position => _position;
        public IReadOnlyList<PointInfo> NextPoints => _nextPoints;
        public VarietiesPoint VarietiesPoint => _varietiesPoint;
        public Sprite PointSprite => _varietiesPoint.PointSprite;
        public PointType PointType => _varietiesPoint.PointType;
    }
}