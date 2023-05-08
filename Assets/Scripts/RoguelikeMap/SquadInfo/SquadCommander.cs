using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using OrderElimination;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.Points.Models;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.SquadInfo
{
    public class SquadCommander
    {
        private readonly IObjectResolver _objectResolver;
        private readonly PanelGenerator _panelGenerator;
        private PointModel _target;
        private Squad _squad;
        public PointModel Target => _target;
        public Squad Squad => _squad;
        public event Action<List<Character>> OnSelected;
        public event Action<int> OnHealAccept;
        public event Action<IReadOnlyList<ItemData>> OnLootAccept;

        [Inject]
        public SquadCommander(IObjectResolver objectResolver, PanelGenerator panelGenerator)
        {
            _objectResolver = objectResolver;
            _panelGenerator = panelGenerator;
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
            battlePanel.OnStartAttack += StartAttack;

            var eventPanel = (EventPanel)_panelGenerator.GetPanelByPointInfo(PointType.Event);
            eventPanel.OnLookForLoot += LootAccept;
            eventPanel.OnStartBattle += StartAttack;

            var shopPanel = (ShopPanel)_panelGenerator.GetPanelByPointInfo(PointType.Shop);
            shopPanel.OnBuyItems += LootAccept;

            var squadMembersPanel = _panelGenerator.GetSquadMembersPanel();
            squadMembersPanel.OnSelected += WereSelectedMembers;
        }

        public void StartAttack()
        {
            if (_target is not BattlePointModel battlePointModel)
                throw new ArgumentException("Is not valid point to attack");
            StartAttack(battlePointModel.Enemies, battlePointModel.MapNumber);
        }

        private void StartAttack(IReadOnlyList<IBattleCharacterInfo> enemies) => StartAttack(enemies, 0);

        private void StartAttack(IReadOnlyList<IBattleCharacterInfo> enemies, int pointNumber)
        {
            if (pointNumber < 0)
                throw new ArgumentOutOfRangeException("Is not valid point number");

            SaveSquadPosition();
            var battleStatsList = _squad.Members.Cast<IBattleCharacterInfo>().ToList();
            var charactersMediator = _objectResolver.Resolve<CharactersMediator>();
            charactersMediator.SetSquad(battleStatsList);
            charactersMediator.SetEnemies(enemies.ToList());
            charactersMediator.SetPointNumber(pointNumber);
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadBattleMap();
        }

        private void SaveSquadPosition()
        {
            PlayerPrefs.SetString(Map.Map.SquadPositionPrefPath, _squad.transform.position.ToString());
        }

        private void WereSelectedMembers(List<Character> characters)
        {
            OnSelected?.Invoke(characters);
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