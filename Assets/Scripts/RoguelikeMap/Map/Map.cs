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
        private TransferPanel _transferPanel;
        private BattlePanel _battlePanel;

        [Inject]
        private void Construct(IMapGenerator mapGenerator, Squad squad,
            ScenesMediator scenesMediator, TransferPanel transferPanel,
            PanelManager panelManager, IObjectResolver objectResolver)
        {
            _mapGenerator = mapGenerator;
            _squad = squad;
            _mediator = scenesMediator;
            _transferPanel = transferPanel;
            _battlePanel = panelManager.GetPanelByPointInfo(PointType.Battle) as BattlePanel;
            _objectResolver = objectResolver;
        }

        private void Start()
        {
            _points = _mapGenerator.GenerateMap();
            _transferPanel.OnAccept += MoveToPoint;
            _battlePanel.OnAccepted += MoveToPoint;
            if(_mediator.Contains<int>("point index"))
                MoveToPoint(_points.First(x => x.Index == _mediator.Get<int>("point index")));
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
            if(_currentPoint is not null)
                _currentPoint.HidePaths();
            _currentPoint = point;
            if (!isAnimation)
                _squad.MoveWithoutAnimation(point.Model.position);
            else if (_mediator.Contains<BattleResults>("battle results")
                     && _mediator.Get<BattleResults>("battle results").BattleOutcome is BattleOutcome.Win)
            {
                if (point.Model is FinalBattlePointModel && point.Model is not BattlePointModel)
                {
                    GameEnd();
                    return;
                }
                _squad.MoveWithoutAnimation(point.Model.position);
                _mediator.Unregister("battle results");
            }
            else
                await point.Visit(_squad);
            UpdatePointsIcon();
            _currentPoint.ShowPaths();
            _mediator.Register("point index", _currentPoint.Index);
        }

        public void LoadStartScene()
        {
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadStartSessionMenu();
        }

        private void UpdatePointsIcon()
        {
            foreach(var point in _points)
                point.Model.SetActive(false);
            
            _currentPoint.Model.SetActive(true);
            var nextPoints = _currentPoint.Model.GetNextPoints();
            if (nextPoints is null)
                return;
            foreach(var point in nextPoints)
                point.SetActive(true);
        }

        public void GameEnd()
        {
            _victoryPanel.Open();
            Destroy(_mediator.gameObject);
        }

        public void OnDestroy()
        {
            _squad.OnVisitPoint -= MoveToPoint;
            _transferPanel.OnAccept -= MoveToPoint;
        }
    }
}