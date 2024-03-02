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
        public event Action<int> OnHealAccept;

        [Inject]
        public SquadCommander(IObjectResolver objectResolver, PanelManager panelManager,
            SquadMembersPanel squadMembersPanel)
        {
            _objectResolver = objectResolver;
            SubscribeToEvents(panelManager);
            _squadMembersPanel = squadMembersPanel;
            squadMembersPanel.OnAttack += StartAttackByBattlePoint;
        }

        public void SetSquad(Squad squad)
        {
            _squad = squad;
        }

        public void SetPoint(PointModel target)
        {
            _target = target;
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
            if (_target is not BattlePointModel battlePointModel)
            {
                Logging.LogException(new ArgumentException("Is not valid point to attack"));
                throw new ArgumentException("Is not valid point to attack");
            }

            StartAttack(battlePointModel.Enemies, battlePointModel.BattleFieldLayout, 
                battlePointModel.BattleRules, battlePointModel.ItemsDropProbability);
        }

        private void StartAttackByEventPoint(BattleNode battleNode)
        {
            if (_target is not EventPointModel eventPointModel)
            {
                Logging.LogException(new ArgumentException("Is not valid point to attack"));
                throw new ArgumentException("Is not valid point to attack");
            }

            StartAttack(battleNode.Enemies, battleNode.BattleFieldLayout, 
                battleNode.BattleRules ,battleNode.ItemsDropProbability);
        }

        private void StartAttack(
            IEnumerable<IGameCharacterTemplate> enemies, IBattleFieldLayout map, BattleRulesPreset battleRules, 
            Dictionary<ItemData, float> items)
        {
            Debug.Log("StartAttack");
            var enemyCharacters = GameCharactersFactory.CreateGameCharacters(enemies);
            var mediator = _objectResolver.Resolve<ScenesMediator>();
            var activeMembers = _squad.ActiveMembers.Where(x => x.CurrentHealth > 0).ToArray();
            mediator.Register(MediatorRegistration.PlayerCharacters, activeMembers);
            mediator.Register(MediatorRegistration.EnemyCharacters, enemyCharacters);
            mediator.Register(MediatorRegistration.BattleField, map);
            var winItems =
                items.Select(it => new KeyValuePair<Item, float>(ItemFactory.Create(it.Key), it.Value))
                    .ToDictionary(x => x.Key, x => x.Value);
            mediator.Register(MediatorRegistration.RewardItems, winItems);
            mediator.Register(MediatorRegistration.CurrentPoint, _target.Index);
            mediator.Register(MediatorRegistration.BattleRules, battleRules);
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadBattleMap();
        }

        private void HealAccept(int amountHeal)
        {
            OnHealAccept?.Invoke(amountHeal);
        }
    }
}