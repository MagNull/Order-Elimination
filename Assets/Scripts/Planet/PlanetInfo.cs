using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "PlanetInfo", menuName = "Planet/New PlanetInfo")]
    public class PlanetInfo : SerializedScriptableObject
    {
        [SerializeField] private Vector3 _position;
        [SerializeField] private Sprite _spriteIcon;
        [SerializeField][Range(0, 1)] private float _сhanceOfItems;
        [SerializeField][Range(0, 1)] private float _chanceOfFighting;
        [SerializeField][Range(0, 1)] private float _chanceOfFightingBack;
        [SerializeField][Range(0, float.MaxValue)] private float _experience;
        [OdinSerialize]
        private List<IBattleCharacterInfo> _enemies;
        
        public Vector3 Position => _position;
        public Sprite SpriteIcon => _spriteIcon;
        public float ChanceOfItems => _сhanceOfItems;
        public float ChanceOfFighting => _chanceOfFighting;
        public float ChanceOfFightingBack => _chanceOfFightingBack;
        public float Experience => _experience;

        public List<IBattleCharacterInfo> Enemies => _enemies;
    }
}