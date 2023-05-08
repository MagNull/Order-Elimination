using Sirenix.OdinInspector;
using UnityEngine;

namespace RoguelikeMap.Points
{
    [CreateAssetMenu(fileName = "PointInfo", menuName = "Point/New PointInfo")]
    public class PointInfo : SerializedScriptableObject
    {
        [SerializeField] 
        private Sprite _sprite;

        [SerializeReference]
        private PointModel _model;

        public Sprite PointSprite => _sprite;
        public PointModel Model => _model;
    }
}