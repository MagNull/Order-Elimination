using UnityEngine;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "PlanetInfo", menuName = "Planet/New PlanetInfo")]
    public class PlanetInfo : ScriptableObject
    {
        [SerializeField] private Sprite _spriteIcon;
        [SerializeField][Range(0, 1)] private float _сhanceOfItems;
        [SerializeField][Range(0, 1)] private float _chanceOfFighting;
        [SerializeField][Range(0, 1)] private float _chanceOfFightingBack;
        [SerializeField][Range(0, float.MaxValue)] private float _expirience;
        
        public Sprite SpriteIcon => _spriteIcon;
        public float ChanceOfItems => _сhanceOfItems;
        public float ChanceOfFighting => _chanceOfFighting;
        public float ChanceOfFightingBack => _chanceOfFightingBack;
        public float Expirience => _expirience;
    }
}