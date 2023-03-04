using System.Linq;
using DG.Tweening;
using RoguelikeMap;
using TMPro;
using UIManagement;
using UIManagement.Panels;
using UnityEngine;
using UnityEngine.UI;
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
            ((EventPanel)UIController.SceneInstance.OpenPanel(PanelType.Event, WindowFormat.Half)).UpdateEventInfo();
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
            ((SafeZonePanel)UIController.SceneInstance.OpenPanel(PanelType.SafeZone, WindowFormat.Half)).UpdateSafeZoneInfo();
        }

        public void StartAttack()
        {
            var battleStatsList = _squad.Members.Cast<IBattleCharacterInfo>().ToList();
            var charactersMediator = _objectResolver.Resolve<CharactersMediator>();
            charactersMediator.SetSquad(battleStatsList);
            charactersMediator.SetEnemies(_target.PointInfo.Enemies);
            charactersMediator.SetPointNumber(_target.PointNumber);
            charactersMediator.PointInfo = _target.PointInfo;
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadBattleMap();
        }
    }
}