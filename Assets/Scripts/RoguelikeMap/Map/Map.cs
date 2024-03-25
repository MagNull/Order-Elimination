using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoguelikeMap.Points;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI.Characters;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.Map
{
    public class Map : MonoBehaviour
    {
        private SimpleMapGenerator _mapGenerator;
        private Squad _squad;
        private List<Point> _points;
        [ShowInInspector]
        private Point _currentPoint;
        private SquadPositionSaver _saver;
        private SquadMembersPanel _squadMembersPanel;

        public PointGraph CurrentMap { get; private set; }

        [Inject]
        private void Construct(SimpleMapGenerator mapGenerator, Squad squad,
            SquadMembersPanel squadMembersPanel, SquadPositionSaver saver)
        {
            _mapGenerator = mapGenerator;
            _squad = squad;
            _squadMembersPanel = squadMembersPanel;
            _saver = saver;
        }

        private void Start()
        {
            _saver.OnSaveBeforeMove += MoveToPoint;
            _saver.OnPassPoint += ShowPaths;
            ShowPaths();
        }

        public void LoadPoints()
        {
            (CurrentMap, _points) = _mapGenerator.GenerateMap();
            HidePointIcons();
        }

        public void ReloadMap()
        {
            SetSquadPosition(FindStartPoint(), false);
        }

        public Point GetPointById(Guid id)
        {
            return id != Guid.Empty ? _points.First(x => x.Id == id) : null;
        }

        private Point FindStartPoint()
        {
            return _points.First(point => point.Model is StartPointModel);
        }

        private async void MoveToPoint(Guid pointId)
        {
            if (pointId != Guid.Empty)
                await SetSquadPosition(_points.First(x => x.Id == pointId));
            else
                ReloadMap();
            if (!_saver.IsPassedPoint())
            {
                _currentPoint.HidePaths();
            }
        }

        public async Task MoveToPoint(Point point)
        {
            if (point is not null)
                await SetSquadPosition(_points.First(x => x.Id == point.Id));
            else
                ReloadMap();
            if (!_saver.IsPassedPoint())
            {
                _currentPoint.HidePaths();
            }
        }

        private async Task SetSquadPosition(Point point, bool isAnimation = true)
        {
            if (_currentPoint is not null)
            {
                _currentPoint.HidePaths();
                _currentPoint.SetActive(false);
            }
            _currentPoint = point;
            await MoveSquad(point, isAnimation);
            _saver.SavePosition(point.Id);
        }

        private async Task MoveSquad(Point point, bool isAnimation)
        {
            if (!isAnimation || _saver.IsPassedPoint())
                _squad.MoveWithoutAnimation(point.Model.position);
            else
                await point.Visit(_squad);
        }

        private void ShowPaths()
        {
            if (_currentPoint != null)
            {
                _currentPoint.ShowPaths();
            }

            _squadMembersPanel.SetActiveAttackButton(false);
        }

        private void HidePointIcons()
        {
            foreach (var point in _points)
                point.Model.SetActive(false);
        }

        public void OnDestroy()
        {
            _saver.OnSaveBeforeMove -= MoveToPoint;
            _saver.OnPassPoint -= ShowPaths;
        }
    }
}