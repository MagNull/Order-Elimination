using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OrderElimination
{
    
    [CreateAssetMenu(fileName = "PlanetInfo", menuName = "Planet/New PlanetInfo")]
    public class PointInfo : SerializedScriptableObject
    {
        [SerializeField]
        private GameObject _pointPrefab;
        [SerializeField]
        private Sprite _spriteIcon;
        [SerializeField]
        private Vector3 _position;
        [SerializeField]
        private List<IPoint> _nextPoints;
        [Range(0, 1)] 
        [SerializeField]
        private float _сhanceOfItems;
        [Range(0, 1)]
        [SerializeField]
        private float _chanceOfFighting;
        [Range(0, 1)]
        [SerializeField]
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
        public GameObject Prefab => _pointPrefab;
        public float ChanceOfItems => _сhanceOfItems;
        public float ChanceOfFighting => _chanceOfFighting;
        public float ChanceOfFightingBack => _chanceOfFightingBack;
        public float Experience => _expirience;
        public int CurrencyReward => _currencyReward;

        public List<IBattleCharacterInfo> Enemies => _enemies;
    }
}