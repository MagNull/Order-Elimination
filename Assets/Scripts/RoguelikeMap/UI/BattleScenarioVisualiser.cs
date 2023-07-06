using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap.UI
{
    public class BattleScenarioVisualiser : SerializedMonoBehaviour
    {
        [SerializeField]
        private List<Sprite> _sprites;
        [OdinSerialize]
        private Dictionary<Vector2Int, Image> _objects;
        [SerializeField]
        private Transform _map;

        private BattleScenario _scenario;
        private bool _isZoom = false;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tweenerCore;

        public float windowScaleCoef = 1.5f;
        public float windowOpeningTime = 0.3f;
        public Ease windowOpeningEase = Ease.Flash;

        [ShowInInspector]
        private const int ExplosibeBattelIndex = 0;
        [ShowInInspector]
        private const int HoleIndex = 1;
        [ShowInInspector]
        private const int LandMineIndex = 2;
        [ShowInInspector]
        private const int ShieldIndex = 3;
        [ShowInInspector]
        private const int SmokeIndex = 4;
        [ShowInInspector]
        private const int StoneIndex = 5;
        [ShowInInspector]
        private const int TowerIndex = 6;
        [ShowInInspector]
        private const int TreeIndex = 7;
        
        public void Initialize(BattleScenario battleScenario,
            IReadOnlyList<GameCharacter> enemies, IReadOnlyList<GameCharacter> allies)
        {
            _scenario = battleScenario;
            UpdateMap(enemies, allies);
        }

        private void UpdateMap(IReadOnlyList<GameCharacter> enemies, IReadOnlyList<GameCharacter> allies)
        {
            var allySpawns = _scenario.GetAlliesSpawnPositions();
            var enemySpawns = _scenario.GetEnemySpawnPositions();
            var structures = _scenario.GetStructureSpawns();
            foreach (var structure in structures)
                UpdateCell(structure.Key, structure.Value.BattleIcon);
            for (var i = 0; i < allies.Count; i++)
                UpdateCell(allySpawns[i], allies[i].CharacterData.BattleIcon);
            for (var i = 0; i < enemies.Count; i++)
                UpdateCell(enemySpawns[i], enemies[i].CharacterData.BattleIcon);
        }

        private void UpdateCell(Vector2Int position, Sprite sprite)
        {
            _objects[position].sprite = sprite;
            _objects[position].gameObject.SetActive(true);
        }

        public void SetActiveCells(bool isActive)
        {
            foreach(var cell in _objects.Values)
                cell.gameObject.SetActive(isActive);
        }

        public void ZoomMap()
        {
            _map.DOComplete();
            _tweenerCore = _map.DOScale(_isZoom ? _map.localScale / windowScaleCoef : _map.localScale * windowScaleCoef,
                windowOpeningTime).SetEase(windowOpeningEase);
            _isZoom = !_isZoom;
        }
    }
}