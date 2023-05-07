﻿using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination
{
    public class CharactersMediator : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private List<IBattleCharacterInfo> _allies;

        [OdinSerialize] private List<IBattleCharacterInfo> _enemies;

        private int _pointNumber;

        [OdinSerialize]
        public BattleScenario BattleScenario { get; set; }

        private void Awake() => DontDestroyOnLoad(gameObject);

        public void SetSquad(List<IBattleCharacterInfo> battleStatsList) => _allies = battleStatsList;
        public void SetEnemies(List<IBattleCharacterInfo> battleStatsList) => _enemies = battleStatsList;

        public void SetPointNumber(int pointNumber) => _pointNumber = pointNumber;

        public List<IBattleCharacterInfo> GetPlayerCharactersInfo() => _allies;
        public List<IBattleCharacterInfo> GetEnemyCharactersInfo() => _enemies;
        public int PointNumber => _pointNumber;

        public PlanetInfo PlanetInfo { get; set; }

    }
}