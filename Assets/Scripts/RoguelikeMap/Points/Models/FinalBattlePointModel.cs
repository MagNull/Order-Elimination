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
        private Squad _squad;

        protected BattlePanel Panel => _panel as BattlePanel;

        public override PointType Type => PointType.Battle;
        public IReadOnlyList<IGameCharacterTemplate> Enemies => _enemies;
        public BattleScenario Scenario => _battleScenario;

        public Dictionary<ItemData, float> ItemsDropProbability => _itemsDropProbability;

        public override void ShowPreview(Squad squad)
        {
            _squad = squad;
            squad.OnUpdateMembers -= Panel.UpdateAlliesOnMap;
            squad.OnUpdateMembers += Panel.UpdateAlliesOnMap;
            _enemiesGameCharacter = GameCharactersFactory.CreateGameCharacters(Enemies).ToList();
            Panel.OnAccepted += MoveToPoint;
            Panel.Initialize(_battleScenario, _enemiesGameCharacter, squad.Members); //TODO: Store GameCharacters
            Panel.Open();
        }

        private void MoveToPoint()
        {
            Panel.OnAccepted -= MoveToPoint;
            _squad.Visit(this);
        }
    }
}