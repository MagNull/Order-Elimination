using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderElimination;
using RoguelikeMap.Points;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI;
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
        private TransferPanel _transferPanel;
        private IObjectResolver _objectResolver;

        [Inject]
        private void Construct(IMapGenerator mapGenerator, Squad squad,
            TransferPanel transferPanel, IObjectResolver objectResolver)
        {
            _mapGenerator = mapGenerator;
            _squad = squad;
            _transferPanel = transferPanel;
            _objectResolver = objectResolver;
        }

        private void Start()
        {
            _points = _mapGenerator.GenerateMap();
            _transferPanel.OnAcceptClick += MoveToPoint; 
            ReloadMap();
        }
        
        public void ReloadMap()
        {
            if(_currentPoint is not null)
                _currentPoint.HidePaths();
            _currentPoint = FindStartPoint();
            _squad.MoveWithoutAnimation(_currentPoint.Model.position);
            UpdatePointsIcon();
            _currentPoint.ShowPaths();
        }

        private Point FindStartPoint()
        {
            return _points.First(point => point.Model is StartPointModel);
        }

        private async void MoveToPoint(Point point)
        {
            await SetSquadPosition(point);
        }

        private async Task SetSquadPosition(Point point)
        {
            if(_currentPoint is not null)
                _currentPoint.HidePaths();
            _currentPoint = point;
            await point.Visit(_squad);
            UpdatePointsIcon();
            _currentPoint.ShowPaths();
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

        public void OnDestroy()
        {
            _transferPanel.OnAcceptClick -= MoveToPoint;
        }
    }
}