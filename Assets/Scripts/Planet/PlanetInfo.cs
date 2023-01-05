using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "PlanetInfo", menuName = "Planet/New PlanetInfo")]
    public class PlanetInfo : ScriptableObject
    {
        [SerializeField] private Vector3 _position;
        [SerializeField] private List<PathInfo> _paths;
        [SerializeField] private Sprite _spriteIcon;
        [SerializeField][Range(0, 1)] private float _сhanceOfItems;
        [SerializeField][Range(0, 1)] private float _chanceOfFighting;
        [SerializeField][Range(0, 1)] private float _chanceOfFightingBack;
        [SerializeField][Range(0, float.MaxValue)] private float _expirience;
        
        public Vector3 Position => _position;
        public IReadOnlyList<PathInfo> Paths => _paths;
        public Sprite SpriteIcon => _spriteIcon;
        public float ChanceOfItems => _сhanceOfItems;
        public float ChanceOfFighting => _chanceOfFighting;
        public float ChanceOfFightingBack => _chanceOfFightingBack;
        public float Expirience => _expirience;
    }
}