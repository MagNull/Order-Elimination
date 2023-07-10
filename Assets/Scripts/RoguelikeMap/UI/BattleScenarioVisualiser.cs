using System.Collections.Generic;
using DG.Tweening;
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

        private BattleScenario _scenario;
        private bool _isZoom = false;

        public float windowScaleCoef = 1.5f;
        public float windowOpeningTime = 0.3f;
        public Ease windowOpeningEase = Ease.Flash;
        
        public void Initialize(BattleScenario battleScenario,
            IReadOnlyList<GameCharacter> enemies, IReadOnlyList<GameCharacter> allies)
        {
            _scenario = battleScenario;
            UpdateMap(enemies, allies);
        }

        private void UpdateMap(IReadOnlyList<GameCharacter> enemies, IReadOnlyList<GameCharacter> allies)
        {
            UpdateCharactersCells(enemies, true);
            UpdateCharactersCells(allies);
            
            var structures = _scenario.GetStructureSpawns();
            foreach (var structure in structures)
                UpdateCell(structure.Key, structure.Value.BattleIcon);
        }

        public void UpdateCharactersCells(IReadOnlyList<GameCharacter> characters, bool isEnemy = false)
        {
            var characterSpawns = isEnemy 
                ? _scenario.GetEnemySpawnPositions() 
                : _scenario.GetAlliesSpawnPositions();
            for (var i = 0; i < characters.Count; i++)
                UpdateCell(characterSpawns[i], characters[i].CharacterData.BattleIcon);
        }

        private void UpdateCell(Vector2Int position, Sprite sprite)
        {
            _objects[position].sprite = sprite;
            _objects[position].gameObject.SetActive(true);
        }

        public void SetActiveAlliesCells(bool isActive)
        {
            if (_scenario is not null)
                SetActiveCells(_scenario.GetAlliesSpawnPositions(), isActive);
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