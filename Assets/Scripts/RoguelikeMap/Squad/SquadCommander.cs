using System;
using System.Collections.Generic;
using System.Linq;
using RoguelikeMap.Map;
using RoguelikeMap.Points;
using UnityEngine;
using VContainer;

namespace OrderElimination
{
    public class SquadCommander
    {
        private readonly IObjectResolver _objectResolver;
        private Point _target;
        private Squad _squad;
        public Point Target => _target;
        public Squad Squad => _squad;
        public event Action<List<Character>> OnSelected;
        public event Action OnHealAccept;

        [Inject]
        public SquadCommander(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public void Set(Squad squad, Point target)
        {
            _squad = squad;
            _target = target;
        }
        
        private void SubscribeToEvents()
        {
            //FullScreen
            // ((ChoosingCharacter)UIController.SceneInstance
            //     .OpenPanel(PanelType.SquadMembers)).OnSelected += OnSelected.Invoke;
            //FullScreen
            // ((SafeZonePanel)UIController.SceneInstance
            //     .OpenPanel(PanelType.SafeZone)).OnHealAccept += OnHealAccept.Invoke;
        }

        public void ShowEventImage()
        {
            //Small
            //((EventPanel)UIController.SceneInstance.OpenPanel(PanelType.Event)).UpdateEventInfo();
        }

        public void ShowSafeZoneImage()
        {
            //Small
            //((SafeZonePanel)UIController.SceneInstance.OpenPanel(PanelType.SafeZone)).UpdateSafeZoneInfo();
        }

        public void ShowSquadMembers()
        {
            //FullScreen
            // ((ChoosingCharacter)UIController.SceneInstance
            //     .OpenPanel(PanelType.SquadMembers))
            //     .UpdateCharacterInfo(_squad.Members, _target is BattlePoint);
        }

        public void StartAttack()
        {
            SaveSquadPosition();
            var battleStatsList = _squad.Members.Cast<IBattleCharacterInfo>().ToList();
            var charactersMediator = _objectResolver.Resolve<CharactersMediator>();
            charactersMediator.SetSquad(battleStatsList);
            charactersMediator.SetEnemies(_target.PointInfo.Enemies);
            charactersMediator.SetPointNumber(_target.PointNumber);
            charactersMediator.PointInfo = _target.PointInfo;
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadBattleMap();
        }

        private void SaveSquadPosition()
        {
            PlayerPrefs.SetString(Map.SquadPositionPrefPath, _squad.transform.position.ToString());
        }
    }
}