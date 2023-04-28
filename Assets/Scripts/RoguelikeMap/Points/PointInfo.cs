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
        private Sprite _sprite;

        [SerializeReference]
        private PointModel _model;

        public Vector3 Position => _position;
        public Sprite PointSprite => _sprite;
        public PointModel Model => _model;
    }
}