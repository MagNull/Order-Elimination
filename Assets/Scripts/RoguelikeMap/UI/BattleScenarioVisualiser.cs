using System.Collections.Generic;
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

        private BattleScenario _scenario;
        
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
    }
}