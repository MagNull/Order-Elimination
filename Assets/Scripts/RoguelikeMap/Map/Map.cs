using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderElimination.Battle;
using RoguelikeMap.Points;
using RoguelikeMap.SquadInfo;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.Map
{
    public class Map : MonoBehaviour
    {
        private IMapGenerator _mapGenerator;
        private Squad _squad;
        private List<Point> _points;
        private Point _currentPoint;
        private BattleOutcome _battleOutcome;
        private SquadPositionSaver _saver;

        [Inject]
        private void Construct(IMapGenerator mapGenerator, Squad squad,
            SquadPositionSaver saver)
        {
            _mapGenerator = mapGenerator;
            _squad = squad;
            _saver = saver;
        }

        private void Start()
        {
            _saver.OnSaveBeforeMove += MoveToPoint;
        }

        public void LoadPoints()
        {
            _points = _mapGenerator.GenerateMap();
            HidePointIcons();
        }

        public void ReloadMap()
        {
            SetSquadPosition(FindStartPoint(), false);
        }

        public Point GetPointByIndex(int pointIndex)
        {
            return pointIndex != -1 ? _points.First(x => x.Index == pointIndex) : null;
        }

        private Point FindStartPoint()
        {
            return _points.First(point => point.Model is StartPointModel);
        }

        private async void MoveToPoint(int pointIndex)
        {
            if (pointIndex > 0 && pointIndex < _points.Count)
                await SetSquadPosition(_points.First(x => x.Index == pointIndex));
            else
                ReloadMap();
        }

        public async Task MoveToPoint(Point point)
        {
            if (point is not null)
                await SetSquadPosition(_points.First(x => x.Index == point.Index));
            else
                ReloadMap();
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
            else
                await point.Visit(_squad);
        }

        private void HidePointIcons()
        {
            foreach (var point in _points)
                point.Model.SetActive(false);
        }

        public void OnDestroy()
        {
            _saver.OnSaveBeforeMove -= MoveToPoint;
        }
    }
}