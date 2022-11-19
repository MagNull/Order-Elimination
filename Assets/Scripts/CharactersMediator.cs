using System;
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

        private void Awake() => DontDestroyOnLoad(gameObject);

        public void SetCharacters(List<IBattleCharacterInfo> battleStatsList) => _battleStatsList = battleStatsList;

        public List<IBattleCharacterInfo> GetBattleCharactersInfo() => _battleStatsList;
    }
}