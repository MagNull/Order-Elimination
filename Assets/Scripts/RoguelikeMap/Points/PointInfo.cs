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
        private VarietiesPointInfo _varietiesPointInfo;
        
        public Vector3 Position => _position;
        public IReadOnlyList<PointInfo> NextPoints => _nextPoints;
        public VarietiesPointInfo VarietiesPointInfo => _varietiesPointInfo;
        public Sprite PointSprite => _varietiesPointInfo.PointSprite;
        public PointType PointType => _varietiesPointInfo.PointType;
    }
}