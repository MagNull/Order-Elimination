using UnityEngine;

namespace RoguelikeMap.Points.VarietiesPoints.Infos
{
    public class VarietiesPoint
    {
        [SerializeField] 
        private Sprite _pointSprite;

        public Sprite PointSprite => _pointSprite;
        public virtual PointType PointType { get; }
    }
}