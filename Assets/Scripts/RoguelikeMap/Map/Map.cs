using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderElimination;
using OrderElimination.Battle;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.Points.Models;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.Map
{
    public class Map : MonoBehaviour
    {
        [SerializeField]
        private Panel _victoryPanel;

        private IMapGenerator _mapGenerator;
        private Squad _squad;
        private List<Point> _points;
        private Point _currentPoint;
        private ScenesMediator _mediator;
        private IObjectResolver _objectResolver;
        private BattleOutcome _battleOutcome;
        private BattlePanel _battlePanel;
        private SquadPositionSaver _saver;

        [Inject]
        private void Construct(IMapGenerator mapGenerator, Squad squad,
            ScenesMediator scenesMediator, PanelManager panelManager, 
            SquadPositionSaver saver, IObjectResolver objectResolver)
        {
            _mapGenerator = mapGenerator;
            _squad = squad;
            _mediator = scenesMediator;
            _battlePanel = panelManager.GetPanelByPointInfo(PointType.Battle) as BattlePanel;
            _saver = saver;
            _objectResolver = objectResolver;
        }

        private void Start()
        {
            _points = _mapGenerator.GenerateMap();
            _saver.OnSaveBeforeMove += MoveToPoint;
            _battlePanel.OnAccepted += MoveToPoint;
            HidePointIcons();
            var pointIndex = _saver.GetPointIndex();
            if(pointIndex != -1)
                MoveToPoint(_points.First(x => x.Index == pointIndex));
            else
                ReloadMap();
        }
        
        public void ReloadMap()
        {
            SetSquadPosition(FindStartPoint(), false);
        }

        private Point FindStartPoint()
        {
            return _points.First(point => point.Model is StartPointModel);
        }

        private async void MoveToPoint(int pointIndex)
        {
            var point = _points.First(x => x.Index == pointIndex);
            await SetSquadPosition(point);
        }

        private async void MoveToPoint(Point point)
        {
            await SetSquadPosition(point);
        }

        private async Task SetSquadPosition(Point point, bool isAnimation = true)
        {
            if (_currentPoint is not null)
                _currentPoint.HidePaths();
            _currentPoint = point;
            await MoveSquad(point, isAnimation);
            _currentPoint.ShowPaths();
            _saver.SavePosition(point.Index);
        }

        private async Task MoveSquad(Point point, bool isAnimation)
        {
            if (!isAnimation || _saver.IsPassedPoint())
                _squad.MoveWithoutAnimation(point.Model.position);
            else if (_mediator.Contains<BattleResults>("battle results")
                     && _mediator.Get<BattleResults>("battle results").BattleOutcome is BattleOutcome.Win)
            {
                if (point.Model is FinalBattlePointModel)
                {
                    GameEnd();
                    return;
                }
                _squad.MoveWithoutAnimation(point.Model.position);
                _mediator.Unregister("battle results");
                _saver.PassPoint();
            }
            else
                await point.Visit(_squad);
        }

        public void LoadStartScene()
        {
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadStartSessionMenu();
        }
        
        private void HidePointIcons()
        {
            foreach(var point in _points)
                point.Model.SetActive(false);
        }

        public void GameEnd()
        {
            _victoryPanel.Open();
            Destroy(_mediator.gameObject);
        }

        public void OnDestroy()
        {
            _saver.OnSaveBeforeMove -= MoveToPoint;
            _battlePanel.OnAccepted -= MoveToPoint;
        }
    }
}