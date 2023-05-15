using OrderElimination.BM;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "Battle Scenario", menuName = "Battle/Battle Scenario")]
    public class BattleScenario : SerializedScriptableObject
    {
        [ShowInInspector, PropertyOrder(100)]
        public IReadOnlyList<Vector2Int> AlliesSpawnPositions => _alliesSpawnPositions;
        [ShowInInspector, PropertyOrder(101)]
        public IReadOnlyList<Vector2Int> EnemySpawnPositions => _enemySpawnPositions;
        [ShowInInspector, PropertyOrder(102)]
        public IReadOnlyDictionary<Vector2Int, EnvironmentInfo> MapObjects => _environmentObjects;

        [SerializeField, HideInInspector]
        private HashSet<Vector2Int> _occupiedSpawnPositions = new HashSet<Vector2Int>();
        [SerializeField, HideInInspector]
        private List<Vector2Int> _alliesSpawnPositions = new List<Vector2Int>();
        [SerializeField, HideInInspector]
        private List<Vector2Int> _enemySpawnPositions = new List<Vector2Int>();
        [SerializeField, HideInInspector]
        private Dictionary<Vector2Int, EnvironmentInfo> _environmentObjects = new Dictionary<Vector2Int, EnvironmentInfo>();

        [TitleGroup("Editing character spawns", BoldTitle = true, Alignment = TitleAlignments.Centered)]
        [Button(DrawResult = false, Style = ButtonStyle.FoldoutButton, Expanded = true)]
        public bool AddAllySpawn(Vector2Int position)
        {
            if (_occupiedSpawnPositions.Contains(position))
                return false;
            _alliesSpawnPositions.Add(position);
            _occupiedSpawnPositions.Add(position);
            return true;
        }

        [Button(DrawResult = false, Style = ButtonStyle.FoldoutButton, Expanded = true)]
        public bool AddEnemySpawn(Vector2Int position)
        {
            if (_occupiedSpawnPositions.Contains(position))
                return false;
            _enemySpawnPositions.Add(position);
            _occupiedSpawnPositions.Add(position);
            return true;
        }

        [Button(DrawResult = false, Style = ButtonStyle.FoldoutButton, Expanded = true)]
        public bool RemoveCharacterSpawn(Vector2Int position)
        {
            if (_alliesSpawnPositions.Remove(position) || _enemySpawnPositions.Remove(position))
            {
                _occupiedSpawnPositions.Remove(position);
                return true;
            }
            return false;
        }

        [TitleGroup("Editing object spawns", BoldTitle = true, Alignment = TitleAlignments.Centered)]
        [Button(DrawResult = false, Style = ButtonStyle.Box, Expanded = true)]
        public bool AddEnvironmentObject(Vector2Int position, EnvironmentInfo objectInfo)
        {
            if (_environmentObjects.ContainsKey(position))
                return false;
            _environmentObjects.Add(position, objectInfo);
            return true;
        }

        [Button(DrawResult = false, Style = ButtonStyle.Box, Expanded = true)]
        public bool RemoveEnvironmentObject(Vector2Int position)
        {
            return _environmentObjects.Remove(position);
        }
    }
}
