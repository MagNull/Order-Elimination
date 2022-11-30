﻿using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace OrderElimination
{
    public class CharactersMediator : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private List<IBattleCharacterInfo> _battleStatsList;

        [OdinSerialize] private List<IBattleCharacterInfo> _enemies;

        private void Awake() => DontDestroyOnLoad(gameObject);

        public void SetCharacters(List<IBattleCharacterInfo> battleStatsList) => _battleStatsList = battleStatsList;

        public List<IBattleCharacterInfo> GetBattleCharactersInfo() => _battleStatsList;
        public List<IBattleCharacterInfo> GetBattleEnemyInfo() => _enemies;
    }
}