using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.MetaGame;
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
        public PointModel Target => _target;
        public Squad Squad => _squad;
        public event Action<List<GameCharacter>, int> OnSelected;
        public event Action<int> OnHealAccept;
        public event Action<IReadOnlyList<ItemData>> OnLootAccept;

        [Inject]
        public SquadCommander(IObjectResolver objectResolver, PanelManager panelManager, SquadMembersPanel squadMembersPanel)
        {
            _objectResolver = objectResolver;
            SubscribeToEvents(panelManager);
            squadMembersPanel.OnSelected += WereSelectedMembers;
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
            safeZonePanel.OnLootAccept += LootAccept;
            safeZonePanel.OnHealAccept += HealAccept;

            var battlePanel = (BattlePanel)panelManager.GetPanelByPointInfo(PointType.Battle);
            battlePanel.OnStartAttack += StartAttackByBattlePoint;

            var eventPanel = (EventPanel)panelManager.GetPanelByPointInfo(PointType.Event);
            eventPanel.OnStartBattle += StartAttackByEventPoint;
        }

        private void StartAttackByBattlePoint()
        {
            if (_target is not BattlePointModel battlePointModel)
                throw new ArgumentException("Is not valid point to attack");
            StartAttack(GameCharactersFactory.CreateGameEntities(battlePointModel.Enemies), battlePointModel.Scenario);
        }
        
        private void StartAttackByEventPoint(IReadOnlyList<IGameCharacterData> enemies)
        {
            if (_target is not EventPointModel eventPointModel)
                throw new ArgumentException("Is not valid point to attack");
            StartAttack(GameCharactersFactory.CreateGameEntities(enemies), eventPointModel.Scenario);
        }
        
        private void StartAttack(IEnumerable<GameCharacter> enemies, BattleScenario scenario)
        {
            SaveSquadPosition();
            var charactersMediator = _objectResolver.Resolve<CharactersMediator>();
            charactersMediator.SetPlayerSquad(_squad.Members);
            charactersMediator.SetEnemies(enemies);
            charactersMediator.SetScenario(scenario);
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadBattleMap();
        }

        private void SaveSquadPosition()
        {
            PlayerPrefs.SetString(Map.Map.SquadPositionPrefPath, _squad.transform.position.ToString());
        }

        private void WereSelectedMembers(List<GameCharacter> characters, int activeMembersCount)
        {
            OnSelected?.Invoke(characters, activeMembersCount);
        }

        private void HealAccept(int amountHeal)
        {
            OnHealAccept?.Invoke(amountHeal);
        }

        //TODO(coder): add loot to player inventory after create inventory system
        private void LootAccept(IReadOnlyList<ItemData> itemsId)
        {
            OnLootAccept?.Invoke(itemsId);
        }
    }
}