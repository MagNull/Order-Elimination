using OrderElimination.Editor;
using OrderElimination.Infrastructure;
using OrderElimination.Utils;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace OrderElimination.Battle
{
    [CreateAssetMenu(fileName = "new BattleMap Layout", menuName = "OrderElimination/Battle/BattleMap Layout")]
    public class BattleMapLayeredLayout : SerializedScriptableObject, IBattleMapLayout
    {
        #region OdinVisuals
        [ShowInInspector, PropertyOrder(-10)]
        private static bool _displayCoordinates = true;
        //[ShowInInspector, PropertyOrder(-10)]
        private static bool _displayBackground = true;

        [TableMatrix(
            DrawElementMethod = nameof(DrawPreviewCell),
            SquareCells = true, HideRowIndices = true, ResizableColumns = false)]
        [Title("Preview", TitleAlignment = TitleAlignments.Centered)]
        [ShowInInspector, OdinSerialize]
        private Vector2Int[,] _preview;

        private Vector2Int DrawPreviewCell(Rect rect, Vector2Int pos)
        {
            var gamePos = InverseY(pos);//Y-axis inversion

            if (Event.current.type == EventType.MouseDown
                && rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 0)
                {
                    var spawningSides = EnumMask<BattleSide>.Empty;
                    if (_spawns.ContainsKey(gamePos))
                    {
                        if (_spawns[gamePos].SpawningSides[BattleSide.Player])
                        {
                            spawningSides[BattleSide.Enemies] = true;
                            _spawns[gamePos] = new(spawningSides);
                        }
                        else if (_spawns[gamePos].SpawningSides[BattleSide.Enemies])
                        {
                            _spawns.Remove(gamePos);
                        }
                    }
                    else
                    {
                        spawningSides[BattleSide.Player] = true;
                        _spawns.Add(gamePos, new(spawningSides));
                    }
                }
                GUI.changed = true;
                Event.current.Use();
            }

#if UNITY_EDITOR
            //Draw Background
            if (BackgroundImage != null && _displayBackground)
            {
                //draw image part
                var texture = BackgroundImage;
                var deltaX = (float)texture.width / Width;
                var deltaY = (float)texture.height / Height;
                var cropRect = new Rect(deltaX * pos.x, deltaY * pos.y, deltaX, deltaY);
                var texturePart = texture.CropTexture(cropRect);
                GUI.DrawTexture(rect, texturePart, ScaleMode.StretchToFill);
            }
            var cellColor = Color.black.WithAlpha(0);
            var cellText = new StringBuilder();
            if (_displayCoordinates)
                cellText.Append(gamePos);
            var structTint = new Color(1, 1, 1, 0.5f);
            var charTint = new Color(1, 1, 1, 0.8f);
            var charRect = rect.AlignCenterXY(rect.width * 0.7f);

            //Draw Spawns
            if (_spawns.ContainsKey(gamePos))
            {
                var sides = EnumExtensions.GetValues<BattleSide>()
                    .Where(s => _spawns[gamePos].SpawningSides[s])
                    .ToArray();
                foreach (var side in sides)
                {
                    if (_spawns[gamePos].SpawningSides[side])
                        cellText.Append($"\n{side} ({_spawns[gamePos].PriorityOrder})");
                    cellColor = side switch
                    {
                        BattleSide.NoSide => new Color(0.7f, 0.7f, 0.7f),
                        BattleSide.Player => cellColor = new Color(0.4f, 1, 0.4f),
                        BattleSide.Enemies => cellColor = new Color(1, 0.4f, 0.4f),
                        BattleSide.Allies => new Color(0.4f, 0.9f, 0.9f),
                        BattleSide.Others => new Color(1, 0.5f, 0),
                        _ => throw new NotImplementedException(),
                    };
                }
                if (sides.Length > 1)
                    cellColor = Color.yellow;
            }
            if (_displayBackground && cellColor.a != 0)
                cellColor = cellColor.WithAlpha(0.8f);
            EditorGUI.DrawRect(rect.AlignCenterXY(rect.width * 0.98f), cellColor);

            //Draw Entities
            foreach (var (item, side) in GetItemsFromLayersAt(_structureLayers, pos.x, pos.y))
            {
                var sprite = item.BattleIcon;
                var texture = sprite.texture;
                if (true)//(sprite.packed)
                {
                    var textRect = item.BattleIcon.rect;
                    var yMax = textRect.yMax;
                    var yMin = textRect.yMin;
                    textRect.yMax = texture.height - 1 - yMin;
                    textRect.yMin = texture.height - 1 - yMax;
                    texture = texture.CropTexture(textRect);
                }
                GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit, true, 0, structTint, 0, 0);
            }
            foreach (var (item, side) in GetItemsFromLayersAt(_characterLayers, pos.x, pos.y))
            {
                var sprite = item.BattleIcon;
                var texture = sprite.texture;
                if (true)//(sprite.packed)
                {
                    var textRect = item.BattleIcon.rect;
                    var yMax = textRect.yMax;
                    var yMin = textRect.yMin;
                    textRect.yMax = texture.height - 1 - yMin;
                    textRect.yMin = texture.height - 1 - yMax;
                    texture = texture.CropTexture(textRect);
                }
                GUI.DrawTexture(charRect, texture, ScaleMode.ScaleToFit, true, 0, charTint, 0, 0);
            }

            //Draw Text data
            InspectorGUIExtensions.DrawLabel(rect, cellText.ToString(), cellColor.GetContrastColor());
#endif
            return pos;
        }
        #endregion

        private int InverseY(int y) => Height - 1 - y;
        private Vector2Int InverseY(Vector2Int pos) => new(pos.x, InverseY(pos.y));

        public BattleMapLayeredLayout()
        {
            var defaultSize = 8;
            _size = defaultSize;
            var positions = EnumerableExtensions.GetIndexVectorMatrix(defaultSize, defaultSize);
            _preview = positions;
        }

        [TitleGroup("Map Properties", Alignment = TitleAlignments.Centered)]
        [PropertyOrder(-1)]
        [ShowInInspector, OdinSerialize]
        public string MapName { get; private set; }

        //[PropertyOrder(-1)]
        //[ShowInInspector, OdinSerialize]
        public Texture BackgroundImage { get; private set; }

        [HideInInspector, OdinSerialize]
        private int _size { get; set; } = 8;

        [ShowInInspector, PropertyOrder(-1)]
        public int Size
        {
            get => _size;
            set
            {
                if (value < 1) value = 1;
                if (value > 20) value = 20;

                if (_size != value
                    && _characterLayers != null
                    && _structureLayers != null)
                {
                    if (!_characterLayers.All(l => l.CanResizeWithoutLoss(value, value))
                        || !_structureLayers.All(l => l.CanResizeWithoutLoss(value, value)))
                    {
                        Logging.LogError("BattleMap Layout Size modification prevented. " +
                            "There are items that can be deleted by this operation. " +
                            "Remove items from truncating area first!");
                        return;
                    }
                    foreach (var layer in _characterLayers)
                    {
                        layer.Resize(value, value);
                    }
                    foreach (var layer in _structureLayers)
                    {
                        layer.Resize(value, value);
                    }
                    var positions = EnumerableExtensions.GetIndexVectorMatrix(value, value);
                    var delta = value - _size;
                    _spawns = _spawns.ToDictionary(kv => kv.Key + Vector2Int.up * delta, kv => kv.Value);
                    _preview = positions;
                }
                else
                {
                    return;
                }
                _size = value;
                //max size limit
            }
        }

        [TitleGroup("Spawns", Alignment = TitleAlignments.Centered)]
        [DictionaryDrawerSettings(KeyLabel = "Position", ValueLabel = "Spawn")]
        [ShowInInspector, OdinSerialize]
        private Dictionary<Vector2Int, BattleMapLayoutSpawn> _spawns = new();

        [TitleGroup("Entities", Alignment = TitleAlignments.Centered)]
        [ListDrawerSettings(CustomAddFunction = nameof(AddStructureLayer))]
        [ShowInInspector, OdinSerialize]
        private List<BattleMapLayoutLayer<StructureTemplate>> _structureLayers = new();

        [PropertySpace(SpaceBefore = 10, SpaceAfter = 0)]
        [TitleGroup("Entities", Alignment = TitleAlignments.Centered)]
        [ListDrawerSettings(CustomAddFunction = nameof(AddSpawnLayer))]
        [ShowInInspector, OdinSerialize]
        private List<BattleMapLayoutLayer<CharacterTemplate>> _characterLayers = new();

        public int Width => Size;
        public int Height => Size;

        public BattleSpawnData[] GetSpawns()
        {
            return _spawns
                .Where(kv => kv.Key.x >= 0 && kv.Key.y >= 0 && kv.Key.x < Width && kv.Key.y < Height)
                .OrderBy(kv => kv.Value.PriorityOrder)
                .Select(kv => new BattleSpawnData(kv.Key, kv.Value.SpawningSides))
                .ToArray();
        }

        public CharacterSpawnData[] GetCharacters()
        {
            var characters = new List<CharacterSpawnData>();
            //per-cell providing
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    foreach (var (item, side) in GetItemsFromLayersAt(_characterLayers, x, y))
                    {
                        characters.Add(new(item, side, new Vector2Int(x, InverseY(y))));
                    }
                }
            }
            return characters.ToArray();
        }

        public StructureSpawnData[] GetStructures()
        {
            var structures = new List<StructureSpawnData>();
            //per-cell providing
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    foreach (var (item, side) in GetItemsFromLayersAt(_structureLayers, x, y))
                    {
                        structures.Add(new(item, side, new Vector2Int(x, InverseY(y))));
                    }
                }
            }
            return structures.ToArray();
        }

        private static (TItem item, BattleSide side)[] GetItemsFromLayersAt<TItem>(
            IEnumerable<BattleMapLayoutLayer<TItem>> layers, int x, int y)
        {
            if (x < 0 || y < 0)
                throw new ArgumentOutOfRangeException();
            if (layers.Any(l => l.Items.GetLength(0) < x || l.Items.GetLength(1) < y))
                throw new ArgumentOutOfRangeException();
            var objects = new List<(TItem, BattleSide)>();
            foreach (var layer in layers)
            {
                if (layer.Items[x, y] != null)
                    objects.Add((layer.Items[x, y], layer.Side));
            }
            return objects.ToArray();
        }

        private bool HasSpawnsOutsideBorders
            => _spawns.Keys.Any(pos => pos.x < 0 || pos.y < 0 || pos.x > Width - 1 || pos.y > Height - 1);

        [TitleGroup("Spawns")]
        [EnableIf("@" + nameof(HasSpawnsOutsideBorders))]
        [Button]
        private void ClearSpawnsOutsideBorders()
        {
            foreach (var pos in _spawns.Keys.ToArray())
            {
                if (pos.x < 0 || pos.y < 0 || pos.x > Width - 1 || pos.y > Height - 1)
                    _spawns.Remove(pos);
            }
        }

        private void AddSpawnLayer() => _characterLayers.Add(new(Width, Height));

        private void AddStructureLayer() => _structureLayers.Add(new(Width, Height));
    }
}
