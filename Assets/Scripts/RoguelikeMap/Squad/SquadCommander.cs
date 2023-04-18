using System;
using System.Collections.Generic;
using System.Linq;
using RoguelikeMap.Map;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.Points.VarietiesPoints;
using UnityEngine;
using VContainer;

namespace OrderElimination
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
        public event Action OnLootAccept;

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
            //safeZonePanel.OnLootAccept += OnLootAccept!.Invoke;
            //safeZonePanel.OnHealAccept += OnHealAccept!.Invoke;

            var squadMembersPanel = (SquadMembersPanel)_panelGenerator.GetPanelByPointInfo(PointType.SquadMembers);
            squadMembersPanel.OnSelected += OnSelected!.Invoke;

            var battlePanel = (BattlePanel)_panelGenerator.GetPanelByPointInfo(PointType.Battle);
            battlePanel.OnStartAttack += StartAttack;
        }

        public void StartAttack()
        {
            if (_target is not BattlePoint battlePoint)
                throw new ArgumentException("Is not valid point to attack");
            SaveSquadPosition();
            var battleStatsList = _squad.Members.Cast<IBattleCharacterInfo>().ToList();
            var charactersMediator = _objectResolver.Resolve<CharactersMediator>();
            charactersMediator.SetSquad(battleStatsList);
            //TODO(coder): enemies is IReadOnlyList<IBattleCharacterInfo> but SetEnemies need List<IBattleCharacterInfo>
            charactersMediator.SetEnemies(battlePoint.Enemies.ToList());
            charactersMediator.SetPointNumber(_target.PointNumber);
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadBattleMap();
        }

        private void SaveSquadPosition()
        {
            PlayerPrefs.SetString(Map.SquadPositionPrefPath, _squad.transform.position.ToString());
        }
    }
}