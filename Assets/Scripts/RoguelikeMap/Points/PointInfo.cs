using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoguelikeMap.Points
{
    [CreateAssetMenu(fileName = "PointInfo", menuName = "Point/New PointInfo")]
    public class PointInfo : SerializedScriptableObject
    {
        [SerializeField]
        private GameObject _pointPrefab;
        [SerializeField]
        private Vector3 _position;
        [SerializeField]
        private List<PointInfo> _nextPoints;
        [SerializeField]
        private List<IBattleCharacterInfo> _enemies;
        [SerializeField]
        private int _currencyReward;
        
        [SerializeField]
        public PointType PointType { get; private set; }
        

        public Vector3 Position => _position;
        public IReadOnlyList<PointInfo> NextPoints => _nextPoints;
        public GameObject Prefab => _pointPrefab;
        public int CurrencyReward => _currencyReward;

        public List<IBattleCharacterInfo> Enemies => _enemies;
    }
}