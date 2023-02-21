using System.Linq;
using DG.Tweening;
using RoguelikeMap;
using TMPro;
using UIManagement;
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
            PlayAnimation();
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
        }

        public void ShowShopImage(DialogWindowData data)
        {
            _window.SetData(data);
            _window.PlayAnimation();
        }

        public void ShowSafeZoneImage(DialogWindowData data)
        {
            _window.SetData(data);
            _window.PlayAnimation();
        }
        
        public void PlayAnimation()
        {
            // var tween = _attackImage.DOFade(1, 0.5f);
            // tween.OnComplete(StartAttack);
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