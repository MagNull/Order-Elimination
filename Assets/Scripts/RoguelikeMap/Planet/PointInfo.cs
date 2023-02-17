using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "PlanetInfo", menuName = "Planet/New PlanetInfo")]
    public class PointInfo : SerializedScriptableObject
    {
        [SerializeField]
        private Vector3 _position;
        [SerializeField]
        private List<IPoint> _nextPoints;
        [SerializeField]
        private Sprite _spriteIcon;
        [SerializeField]
        [Range(0, 1)]
        private float _сhanceOfItems;
        [SerializeField]
        [Range(0, 1)]
        private float _chanceOfFighting;
        [SerializeField]
        [Range(0, 1)]
        private float _chanceOfFightingBack;
        [SerializeField]
        private List<IBattleCharacterInfo> _enemies;

        [SerializeField]
        [Range(0, float.MaxValue)]
        private float _expirience;
        [SerializeField]
        private int _currencyReward;

        public Vector3 Position => _position;
        public IReadOnlyList<IPoint> NextPoints => _nextPoints;
        public Sprite SpriteIcon => _spriteIcon;
        public float ChanceOfItems => _сhanceOfItems;
        public float ChanceOfFighting => _chanceOfFighting;
        public float ChanceOfFightingBack => _chanceOfFightingBack;
        public float Experience => _expirience;
        public int CurrencyReward => _currencyReward;

        public List<IBattleCharacterInfo> Enemies => _enemies;
    }
}