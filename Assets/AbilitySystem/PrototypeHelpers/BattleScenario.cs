using OrderElimination.BM;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "Battle Scenario", menuName = "Battle/Battle Scenario")]
    public class BattleScenario : SerializedScriptableObject
    {
        public const int MapHeight = 8;
        public const int MapWidth = 8;

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

        [TableMatrix(DrawElementMethod = nameof(DrawSpawnCell), SquareCells = true, HideRowIndices = true)]
        //[OnValueChanged(nameof(UpdateMapPreview))]
        [ShowInInspector]
        private SpawnType?[,] _spawnLayout;

        [OnInspectorInit]
        private void UpdateMapPreview()
        {
            _spawnLayout = new SpawnType?[MapWidth, MapHeight];
            foreach (var pos in _entitiesSpawns.Keys)
            {
                _spawnLayout[pos.x, MapHeight - 1 - pos.y] = _entitiesSpawns[pos];
            }
        }

        private SpawnType? DrawSpawnCell(Rect rect, SpawnType? spawnType)
        {
            if (Event.current.type == EventType.MouseDown
                && rect.Contains(Event.current.mousePosition))
            {
                spawnType = spawnType switch
                {
                    SpawnType.Allies => SpawnType.Enemies,
                    SpawnType.Enemies => null,
                    null => SpawnType.Allies,
                    _ => throw new NotImplementedException(),
                };
                GUI.changed = true;
                Event.current.Use();
                Debug.Log("Update cell" % Colorize.Red);
            }
            var cellColor = new Color(0, 0, 0, 0);
            if (spawnType != null)
            {
                var col = GetSpawnTypeColor(spawnType.Value);
                cellColor = new Color(col.r, col.g, col.b, 1f);
                
            }
            EditorGUI.DrawRect(rect, cellColor);
            return spawnType;
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

        public void UpdateSpawns(SpawnType?[,] spawns)
        {
            _entitiesSpawns.Clear();
            for (var x = 0; x < spawns.GetLength(0); x++)
            {
                for (var y = 0; y < spawns.GetLength(1); y++)
                {
                    if (spawns[x, y] != null)
                        _entitiesSpawns.Add(new Vector2Int(x, y), spawns[x, y].Value);
                }
            }
        }

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
        public IReadOnlyDictionary<Vector2Int, EnvironmentInfo> GetStructureSpawns() => _structureSpawns;
    }
}
