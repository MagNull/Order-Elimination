using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.Points.VarietiesPoints;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.SquadInfo
{
    public class SquadCommander
    {
        private readonly IObjectResolver _objectResolver;
        private PanelGenerator _panelGenerator;
        private Point _target;
        private Squad _squad;
        public Point Target => _target;
        public Squad Squad => _squad;
        public event Action<List<Character>> OnSelected;
        public event Action OnHealAccept;
        public event Action<IReadOnlyList<int>> OnLootAccept;

        [Inject]
        public SquadCommander(IObjectResolver objectResolver, PanelGenerator panelGenerator)
        {
            _objectResolver = objectResolver;
            _panelGenerator = panelGenerator;
            _panelGenerator.OnInitializedPanels += SubscribeToEvents;
        }

        public void Set(Squad squad, Point target)
        {
            _squad = squad;
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

            var squadMembersPanel = _panelGenerator.GetSquadMembersPanel();
            squadMembersPanel.OnSelected += WereSelectedMembers;
        }

        public void StartAttack()
        {
            if (_target is not BattlePoint battlePoint)
                throw new ArgumentException("Is not valid point to attack");
            StartAttack(battlePoint.Enemies, _target.PointNumber);
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

        private void HealAccept()
        {
            OnHealAccept?.Invoke();   
        }
        
        //TODO(coder): add loot to player inventory after create inventory system
        private void LootAccept(IReadOnlyList<int> itemsId)
        {
            OnLootAccept?.Invoke(itemsId);
        }
    }
}