using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using OrderElimination;
using OrderElimination.Battle;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap.UI
{
    public class BattleScenarioVisualiser : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private Dictionary<Vector2Int, Image> _objects;
        [SerializeField]
        private Transform _map;

        private IBattleMapLayout _mapLayout;
        private bool _isZoom = false;

        public float windowScaleCoef = 1.5f;
        public float windowOpeningTime = 0.3f;
        public Ease windowOpeningEase = Ease.Flash;
        
        public void Initialize(IBattleMapLayout mapLayout,
            IReadOnlyList<GameCharacter> enemies, IReadOnlyList<GameCharacter> allies)
        {
            _mapLayout = mapLayout;
            UpdateMap(enemies, allies);
        }

        private void UpdateMap(IReadOnlyList<GameCharacter> enemies, IReadOnlyList<GameCharacter> allies)
        {
            UpdateCharactersCells(enemies, BattleSide.Enemies);
            UpdateCharactersCells(allies, BattleSide.Allies);

            var structuresByPos = _mapLayout.GetStructures()
                .GroupBy(s => s.Position)
                .ToDictionary(g => g.Key, g => g.ToArray());
            foreach (var pos in structuresByPos.Keys)
            {
                if (structuresByPos[pos].Length > 1)
                    Logging.LogError(
                        $"Multiple objects at the same position are not supported by {nameof(BattleScenarioVisualiser)}.");
                UpdateCell(pos, structuresByPos[pos][0].StructureTemplate.BattleIcon);
            }
        }

        public void UpdateCharactersCells(IReadOnlyList<GameCharacter> characters, BattleSide side)
        {
            var charactersToSpawn = new Queue<GameCharacter>(characters);
            foreach (var spawn in _mapLayout.GetSpawns())
            {
                if (spawn.SpawningSides[side])
                {
                    UpdateCell(
                        spawn.Position, charactersToSpawn.Dequeue().CharacterData.BattleIcon);
                }
            }
        }

        private void UpdateCell(Vector2Int position, Sprite sprite)
        {
            _objects[position].sprite = sprite;
            _objects[position].gameObject.SetActive(true);
        }

        public void SetActiveAlliesCells(bool isActive)
        {
            if (_mapLayout is not null)
                SetActiveCells(
                    _mapLayout.GetSpawns()
                    .Where(s => s.SpawningSides[BattleSide.Allies])
                    .Select(s => s.Position)
                    .ToList(), 
                    isActive);
        }

        private void SetActiveCells(IReadOnlyList<Vector2Int> positions, bool isActive)
        {
            foreach(var position in positions)
                _objects[position].gameObject.SetActive(isActive);
        }

        public void SetActiveCells(bool isActive)
        {
            foreach(var cell in _objects.Values)
                cell.gameObject.SetActive(isActive);
        }

        public void ZoomMap()
        {
            _map.DOComplete();
            _map.DOScale(_isZoom ? _map.localScale / windowScaleCoef : _map.localScale * windowScaleCoef,
                windowOpeningTime).SetEase(windowOpeningEase);
            _isZoom = !_isZoom;
        }
    }
}