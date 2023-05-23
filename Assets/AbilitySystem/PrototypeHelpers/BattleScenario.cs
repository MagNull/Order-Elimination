using OrderElimination.BM;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
        #region OdinVisuals
        private static Color GetSpawnTypeColor(SpawnType type)
        {
            return type switch
            {
                SpawnType.Allies => Color.green,
                SpawnType.Enemies => Color.red,
                _ => throw new NotImplementedException(),
            };
        }
        #endregion

        [GUIColor("@BattleScenario.GetSpawnTypeColor($value)")]
        public enum SpawnType
        {
            Allies,
            Enemies,
            //Both
        }

        [TitleGroup("Editing character spawns", BoldTitle = true, Alignment = TitleAlignments.Centered)]
        [DictionaryDrawerSettings(KeyLabel = "Position", ValueLabel = "Spawn Type")]
        [ShowInInspector, OdinSerialize]
        private Dictionary<Vector2Int, SpawnType> _entitiesSpawns = new();

        [TitleGroup("Editing structures spawns", BoldTitle = true, Alignment = TitleAlignments.Centered)]
        [DictionaryDrawerSettings(KeyLabel = "Position", ValueLabel = "Structure")]
        [ShowInInspector, OdinSerialize]
        private Dictionary<Vector2Int, EnvironmentInfo> _structureSpawns = new();

        public IReadOnlyDictionary<Vector2Int, EnvironmentInfo> StructureSpawns => _structureSpawns;

        public Vector2Int[] GetAlliesSpawnPositions()
        {
            return _entitiesSpawns.Keys.Where(p => IsRequiredSpawn(p)).ToArray();

            bool IsRequiredSpawn(Vector2Int position)
            {
                return _entitiesSpawns[position] switch
                {
                    SpawnType.Allies => true,
                    SpawnType.Enemies => false,
                    _ => throw new NotImplementedException(),
                };
            }
        }
        public Vector2Int[] GetEnemySpawnPositions()
        {
            return _entitiesSpawns.Keys.Where(p => IsRequiredSpawn(p)).ToArray();

            bool IsRequiredSpawn(Vector2Int position)
            {
                return _entitiesSpawns[position] switch
                {
                    SpawnType.Allies => false,
                    SpawnType.Enemies => true,
                    _ => throw new NotImplementedException(),
                };
            }
        }
    }
}
