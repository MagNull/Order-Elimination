using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "PointInfo", menuName = "Point/New PointInfo")]
    public class PointInfo : SerializedScriptableObject
    {
        [SerializeField]
        private GameObject _pointPrefab;
        [SerializeField]
        private Vector3 _position;
        [SerializeField]
<<<<<<< HEAD:Assets/Scripts/RoguelikeMap/Point/PointInfo.cs
        private List<Point> _nextPoints;
=======
        private List<PointInfo> _nextPoints;
>>>>>>> MapGeneration:Assets/Scripts/RoguelikeMap/Planet/PointInfo.cs
        [Range(0, 1)] 
        [SerializeField]
        private float _сhanceOfItems;
        [SerializeField]
        private List<IBattleCharacterInfo> _enemies;
        [SerializeField]
        [Range(0, float.MaxValue)]
        private float _expirience;
        [SerializeField]
        private int _currencyReward;

        public Vector3 Position => _position;
<<<<<<< HEAD:Assets/Scripts/RoguelikeMap/Point/PointInfo.cs
        public IReadOnlyList<Point> NextPoints => _nextPoints;
        public Sprite SpriteIcon => _spriteIcon;
=======
        public IReadOnlyList<PointInfo> NextPoints => _nextPoints;
>>>>>>> MapGeneration:Assets/Scripts/RoguelikeMap/Planet/PointInfo.cs
        public GameObject Prefab => _pointPrefab;
        public float ChanceOfItems => _сhanceOfItems;
        public float Experience => _expirience;
        public int CurrencyReward => _currencyReward;

        public List<IBattleCharacterInfo> Enemies => _enemies;
    }
}