using System;
using System.Collections.Generic;
using System.Linq;
using RoguelikeMap;
using RoguelikeMap.Point;
using UIManagement;
using UIManagement.Panels;
using UnityEngine;
using VContainer;

namespace OrderElimination
{
    public class SquadCommander
    {
        private readonly IObjectResolver _objectResolver;
        private DialogWindow _window;
        private Point _target;
        private Squad _squad;
        public Point Target => _target;
        public Squad Squad => _squad;
        public event Action<List<Character>> OnSelected;
        public event Action OnHealAccept;

        [Inject]
        public SquadCommander(IObjectResolver objectResolver, DialogWindow window)
        {
            _objectResolver = objectResolver;
            _window = window;
        }

        public void Set(Squad squad, Point target)
        {
            _squad = squad;
            _target = target;
        }
        
        private void SubscribeToEvents()
        {
            ((ChoosingCharacter)UIController.SceneInstance
                .OpenPanel(PanelType.SquadMembers, WindowFormat.FullScreen)).OnSelected += OnSelected.Invoke;
            ((SafeZonePanel)UIController.SceneInstance
                .OpenPanel(PanelType.SafeZone, WindowFormat.FullScreen)).OnHealAccept += OnHealAccept.Invoke;
        }

        public void ShowBattleImage(DialogWindowData data)
        {
            _window.SetData(data);
            _window.PlayAnimation();
            ((SquadListPanel)UIController.SceneInstance.OpenPanel(PanelType.SquadList)).UpdateSquadInfo(Target.PointInfo.Enemies);
        }

        public void ShowEventImage(DialogWindowData data)
        {
            _window.SetData(data);
            _window.PlayAnimation();
            ((EventPanel)UIController.SceneInstance.OpenPanel(PanelType.Event, WindowFormat.Small)).UpdateEventInfo();
        }

        public void ShowShopImage(DialogWindowData data)
        {
            _window.SetData(data);
            _window.PlayAnimation();
            ((ShopPanel)UIController.SceneInstance.OpenPanel(PanelType.Shop, WindowFormat.FullScreen)).UpdateShopInfo();
        }

        public void ShowSafeZoneImage(DialogWindowData data)
        {
            _window.SetData(data);
            _window.PlayAnimation();
            ((SafeZonePanel)UIController.SceneInstance.OpenPanel(PanelType.SafeZone, WindowFormat.Small)).UpdateSafeZoneInfo();
        }

        public void ShowSquadMembers()
        {
            ((ChoosingCharacter)UIController.SceneInstance
                .OpenPanel(PanelType.SquadMembers, WindowFormat.FullScreen))
                .UpdateCharacterInfo(_squad.Members, _target is BattlePoint);
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