using UnityEngine;

namespace RoguelikeMap.Points.VarietiesPoints.Infos
{
    public class VarietiesPointInfo
    {
        [SerializeField] 
        private Sprite _pointSprite;

        public Sprite PointSprite => _pointSprite;
        public virtual PointType PointType { get; }
    }
}