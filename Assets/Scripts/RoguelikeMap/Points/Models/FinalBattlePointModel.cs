using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameInventory.Items;
using OrderElimination;
using OrderElimination.MacroGame;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI.PointPanels;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Rendering;

namespace RoguelikeMap.Points.Models
{
    public class FinalBattlePointModel : PointModel
    {
        [Input] public PointModel entries;
        
        [SerializeField]
        private List<CharacterTemplate> _enemies;
        [SerializeField]
        private BattleScenario _battleScenario;

        [SerializeField]
        private SerializedDictionary<ItemData, float> _itemsDropProbability;

        private List<GameCharacter> _enemiesGameCharacter;

        protected BattlePanel Panel => _panel as BattlePanel;

        public override PointType Type => PointType.Battle;
        public IReadOnlyList<IGameCharacterTemplate> Enemies => _enemies;
        public BattleScenario Scenario => _battleScenario;

        public Dictionary<ItemData, float> ItemsDropProbability => _itemsDropProbability;

        public override async Task Visit(Squad squad)
        {
            squad.OnUpdateMembers -= Panel.UpdateAlliesOnMap;
            squad.OnUpdateMembers += Panel.UpdateAlliesOnMap;
            await base.Visit(squad);
            _enemiesGameCharacter = GameCharactersFactory.CreateGameCharacters(Enemies).ToList();
            Panel.Initialize(_battleScenario, _enemiesGameCharacter, squad.Members); //TODO: Store GameCharacters
            Panel.Open();
        }
    }
}