using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using OrderElimination;
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
        private readonly PanelGenerator _panelGenerator;
        private readonly SquadMembersPanel _squadMembersPanel;
        private PointModel _target;
        private Squad _squad;
        public PointModel Target => _target;
        public Squad Squad => _squad;
        public event Action<List<Character>, int> OnSelected;
        public event Action<int> OnHealAccept;
        public event Action<IReadOnlyList<ItemData>> OnLootAccept;

        [Inject]
        public SquadCommander(IObjectResolver objectResolver, PanelGenerator panelGenerator,
            SquadMembersPanel squadMembersPanel)
        {
            _objectResolver = objectResolver;
            _panelGenerator = panelGenerator;
            _squadMembersPanel = squadMembersPanel;
            _panelGenerator.OnInitializedPanels += SubscribeToEvents;
        }

        public void SetSquad(Squad squad)
        {
            _squad = squad;
        }

        public void SetPoint(PointModel target)
        {
            _target = target;
        }

        private void SubscribeToEvents()
        {
            var safeZonePanel = (SafeZonePanel)_panelGenerator.GetPanelByPointInfo(PointType.SafeZone);
            safeZonePanel.OnLootAccept += LootAccept;
            safeZonePanel.OnHealAccept += HealAccept;

            var battlePanel = (BattlePanel)_panelGenerator.GetPanelByPointInfo(PointType.Battle);
            battlePanel.OnStartAttack += StartAttackByBattlePoint;

            var eventPanel = (EventPanel)_panelGenerator.GetPanelByPointInfo(PointType.Event);
            eventPanel.OnStartBattle += StartAttackByEventPoint;

            _squadMembersPanel.OnSelected += WereSelectedMembers;
        }

        private void StartAttackByBattlePoint()
        {
            if (_target is not BattlePointModel battlePointModel)
                throw new ArgumentException("Is not valid point to attack");
            StartAttack(battlePointModel.Enemies, battlePointModel.Scenario);
        }
        
        private void StartAttackByEventPoint(IReadOnlyList<IBattleCharacterInfo> enemies)
        {
            if (_target is not EventPointModel eventPointModel)
                throw new ArgumentException("Is not valid point to attack");
            StartAttack(enemies, eventPointModel.Scenario);
        }
        
        private void StartAttack(IReadOnlyList<IBattleCharacterInfo> enemies, BattleScenario scenario)
        {
            SaveSquadPosition();
            var battleStatsList = _squad.Members.Cast<IBattleCharacterInfo>().ToList();
            var charactersMediator = _objectResolver.Resolve<CharactersMediator>();
            charactersMediator.SetSquad(battleStatsList);
            charactersMediator.SetEnemies(enemies.ToList());
            charactersMediator.SetScenario(scenario);
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadBattleMap();
        }

        private void SaveSquadPosition()
        {
            PlayerPrefs.SetString(Map.Map.SquadPositionPrefPath, _squad.transform.position.ToString());
        }

        private void WereSelectedMembers(List<Character> characters, int activeMembersCount)
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