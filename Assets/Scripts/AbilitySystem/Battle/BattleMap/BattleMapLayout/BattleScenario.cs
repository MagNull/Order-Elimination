using Mono.Cecil.Cil;
using OrderElimination.Battle;
using OrderElimination.Editor;
using OrderElimination.Infrastructure;
using OrderElimination.Utils;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace OrderElimination.MacroGame
{
    [Obsolete(nameof(BattleScenario) + " deprecated. Use " + nameof(BattleMapLayeredLayout) + " instead.")]
    [CreateAssetMenu(fileName = "new BattleScenario", menuName = "OrderElimination/Battle/Battle Scenario")]
    public class BattleScenario : SerializedScriptableObject, IBattleMapLayout
    {
        public int Height => 8;
        public int Width => 8;

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

        private int InverseY(int y) => Height - 1 - y;

        private readonly struct SpawnInfo
        {
            public readonly Vector2Int Position;
            public readonly SpawnType? SpawnType;

            public SpawnInfo(Vector2Int position, SpawnType? spawnType)
            {
                Position = position;
                SpawnType = spawnType;
            }
        }

        [TitleGroup("Editing character spawns", BoldTitle = true, Alignment = TitleAlignments.Centered)]
        [TableMatrix(DrawElementMethod = nameof(DrawEntitySpawnCell), SquareCells = true, HideRowIndices = true,
            ResizableColumns = false)]
        [OnValueChanged(nameof(UpdateEntitySpawns))]
        [ShowInInspector]
        private SpawnInfo[,] _entitiesSpawnsLayout;

        [TitleGroup("Editing structures spawns", BoldTitle = true, Alignment = TitleAlignments.Centered)]
        [TableMatrix(SquareCells = true, HideRowIndices = true, ResizableColumns = false)]
        [OnValueChanged(nameof(UpdateStructureSpawns))]
        [ShowInInspector]
        private StructureTemplate[,] _structureSpawnsLayout;

        [OnInspectorInit]
        private void UpdateMapPreview()
        {
            if (_entitiesSpawns == null)
                _entitiesSpawns = new();
            if (_structureSpawns == null)
                _structureSpawns = new();
            _entitiesSpawnsLayout = new SpawnInfo[Width, Height];
            for (var x = 0; x < _entitiesSpawnsLayout.GetLength(0); x++)
            {
                for (var y = 0; y < _entitiesSpawnsLayout.GetLength(1); y++)
                {
                    _entitiesSpawnsLayout[x, y] = new SpawnInfo(new Vector2Int(x, InverseY(y)), null);
                }
            }

            foreach (var pos in _entitiesSpawns.Keys)
            {
                var spawnInfo = new SpawnInfo(pos, _entitiesSpawns[pos]);
                _entitiesSpawnsLayout[pos.x, InverseY(pos.y)] = spawnInfo;
            }

            _structureSpawnsLayout = new StructureTemplate[Width, Height];
            foreach (var pos in _structureSpawns.Keys)
            {
                _structureSpawnsLayout[pos.x, InverseY(pos.y)] = _structureSpawns[pos];
            }
        }

        private SpawnInfo DrawEntitySpawnCell(Rect rect, SpawnInfo spawnInfo)
        {
            if (Event.current.type == EventType.MouseDown
                && rect.Contains(Event.current.mousePosition))
            {
                SpawnType? spawnType = spawnInfo.SpawnType switch
                {
                    SpawnType.Allies => SpawnType.Enemies,
                    SpawnType.Enemies => null,
                    null => SpawnType.Allies,
                    _ => throw new NotImplementedException(),
                };
                spawnInfo = new SpawnInfo(spawnInfo.Position, spawnType);
                GUI.changed = true;
                Event.current.Use();
            }

            var cellColor = new Color(0, 0, 0, 0);
            var cellText = new StringBuilder(); //"-";
            cellText.Append(spawnInfo.Position);
            if (spawnInfo.SpawnType != null)
            {
                var col = GetSpawnTypeColor(spawnInfo.SpawnType.Value);
                cellColor = new Color(col.r, col.g, col.b, 1f);
                cellText.Append($"\n{spawnInfo.SpawnType.Value}");
                //cellText += $"\n{spawnInfo.Position}";
            }
#if UNITY_EDITOR
            EditorGUI.DrawRect(rect, cellColor);
#endif
            if (_structureSpawns.ContainsKey(spawnInfo.Position))
            {
                var structure = _structureSpawns[spawnInfo.Position];
                DrawStructureSpawnCell(rect, structure);
            }
            InspectorGUIExtensions.DrawLabel(rect, cellText.ToString(), cellColor.GetContrastColor());

            return spawnInfo;
        }

        private IBattleStructureTemplate DrawStructureSpawnCell(Rect rect, IBattleStructureTemplate structure)
        {
            var tint = new Color(1, 1, 1, 0.5f);
            if (structure != null && structure.BattleIcon != null)
                GUI.DrawTexture(rect, structure.BattleIcon.texture, ScaleMode.ScaleToFit, true, 0, tint, 0, 0);
            return structure;
        }

        private void UpdateEntitySpawns(SpawnInfo[,] spawns)
        {
            _entitiesSpawns.Clear();
            for (var x = 0; x < spawns.GetLength(0); x++)
            {
                for (var y = 0; y < spawns.GetLength(1); y++)
                {
                    if (spawns[x, y].SpawnType != null)
                        _entitiesSpawns.Add(new Vector2Int(x, Height - 1 - y), spawns[x, y].SpawnType.Value);
                }
            }
        }

        private void UpdateStructureSpawns(StructureTemplate[,] spawns)
        {
            _structureSpawns.Clear();
            for (var x = 0; x < spawns.GetLength(0); x++)
            {
                for (var y = 0; y < spawns.GetLength(1); y++)
                {
                    if (spawns[x, y] != null)
                        _structureSpawns.Add(new Vector2Int(x, Height - 1 - y), spawns[x, y]);
                }
            }
        }

        #endregion

        [GUIColor("@BattleScenario.GetSpawnTypeColor($value)")]
        public enum SpawnType
        {
            Allies,
            Enemies,
            //Both
        }

        [HideInInspector, OdinSerialize]
        private Dictionary<Vector2Int, SpawnType> _entitiesSpawns = new();

        [HideInInspector, OdinSerialize]
        private Dictionary<Vector2Int, StructureTemplate> _structureSpawns = new();

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

        public IReadOnlyDictionary<Vector2Int, IBattleStructureTemplate> GetStructureSpawns()
            => _structureSpawns.ToDictionary(kv => kv.Key, kv => (IBattleStructureTemplate)kv.Value);

        public BattleSpawnData[] GetSpawns()
        {
            var allyMask = EnumMask<BattleSide>.Empty;
            allyMask[BattleSide.Player] = true;
            allyMask[BattleSide.Allies] = true;
            var enemyMask = EnumMask<BattleSide>.Empty;
            allyMask[BattleSide.Enemies] = true;
            allyMask[BattleSide.Others] = true;
            var allySpawns = GetAlliesSpawnPositions().Select(p => new BattleSpawnData(p, allyMask.Clone()));
            var enemySpawns = GetEnemySpawnPositions().Select(p => new BattleSpawnData(p, enemyMask.Clone()));
            return allySpawns.Concat(enemySpawns).ToArray();
        }

        public StructureSpawnData[] GetStructures()
        {
            return GetStructureSpawns()
                .Select(kv => new StructureSpawnData(kv.Value, BattleSide.NoSide, kv.Key))
                .ToArray();
        }

        public CharacterSpawnData[] GetCharacters()
        {
            return new CharacterSpawnData[0];
        }
    }
}