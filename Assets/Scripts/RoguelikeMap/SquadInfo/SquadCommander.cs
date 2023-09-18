using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using GameInventory.Items;
using OrderElimination;
using OrderElimination.Battle;
using OrderElimination.MacroGame;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.Points.Models;
using RoguelikeMap.UI.Characters;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.SquadInfo
{
    public class SquadCommander
    {
        private readonly IObjectResolver _objectResolver;
        private PointModel _target;
        private Squad _squad;
        private SquadMembersPanel _squadMembersPanel;

        public BattleOutcome? BattleOutcome { get; private set; } = null;
        public PointModel Target => _target;
        public Squad Squad => _squad;
        public event Action<List<GameCharacter>, int> OnSelected;
        public event Action<int> OnHealAccept;

        [Inject]
        public SquadCommander(IObjectResolver objectResolver, PanelManager panelManager,
            SquadMembersPanel squadMembersPanel)
        {
            _objectResolver = objectResolver;
            SubscribeToEvents(panelManager);
            _squadMembersPanel = squadMembersPanel;
            squadMembersPanel.OnSelected += WereSelectedMembers;
        }

        public void SetSquad(Squad squad)
        {
            _squad = squad;
        }

        public void SetPoint(PointModel target)
        {
            _target = target;
            if(target is FinalBattlePointModel)
                StartAttackByBattlePoint();
        }

        private void SubscribeToEvents(PanelManager panelManager)
        {
            var safeZonePanel = (SafeZonePanel)panelManager.GetPanelByPointInfo(PointType.SafeZone);
            safeZonePanel.OnHealAccept += HealAccept;

            var eventPanel = (EventPanel)panelManager.GetPanelByPointInfo(PointType.Event);
            eventPanel.OnStartBattle += StartAttackByEventPoint;
        }

        private void StartAttackByBattlePoint()
        {
            if (_target is not FinalBattlePointModel battlePointModel)
            {
                Logging.LogException(new ArgumentException("Is not valid point to attack"));
                throw new ArgumentException("Is not valid point to attack");
            }

            StartAttack(battlePointModel.Enemies, battlePointModel.Scenario, battlePointModel.ItemsDropProbability);
        }

        private void StartAttackByEventPoint(BattleNode battleNode)
        {
            if (_target is not EventPointModel eventPointModel)
            {
                Logging.LogException(new ArgumentException("Is not valid point to attack"));
                throw new ArgumentException("Is not valid point to attack");
            }

            StartAttack(battleNode.Enemies, battleNode.Scenario, battleNode.ItemsDropProbability);
        }

        private void StartAttack(
            IEnumerable<IGameCharacterTemplate> enemies, BattleScenario scenario, Dictionary<ItemData, float> items)
        {
            Debug.Log("StartAttack");
            _squadMembersPanel.OnSelected -= WereSelectedMembers;
            var enemyCharacters = GameCharactersFactory.CreateGameCharacters(enemies);
            var mediator = _objectResolver.Resolve<ScenesMediator>();
            mediator.Register("player characters", _squad.Members);
            mediator.Register("enemy characters", enemyCharacters);
            mediator.Register("scenario", scenario);
            var winItems =
                items.Select(it => new KeyValuePair<Item, float>(ItemFactory.Create(it.Key), it.Value))
                    .ToDictionary(x => x.Key, x => x.Value);
            mediator.Register("items", winItems);
            mediator.Register("point index", _target.Index);
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadBattleMap();
        }

        private void WereSelectedMembers(List<GameCharacter> characters, int activeMembersCount)
        {
            OnSelected?.Invoke(characters, activeMembersCount);
        }

        private void HealAccept(int amountHeal)
        {
            OnHealAccept?.Invoke(amountHeal);
        }
    }
}