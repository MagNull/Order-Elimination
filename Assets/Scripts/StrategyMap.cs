using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using OrderElimination.Start;
using Unity.VisualScripting;
using Random = System.Random;
using Vector3 = UnityEngine.Vector3;

namespace OrderElimination
{
    public class StrategyMap : MonoBehaviour
    {
        [SerializeField] private Creator _creator;
        private PlanetInfo[] _pointsInfo;
        private List<PlanetPoint> _planetPoints;
        private List<Squad> _squads;
        private List<Path> _paths;
        private EnemySquad _enemySquad;
        public const float IconSize = 50f;
        public static event Action Onclick;
        public static int CountMove { get; private set; }

        public static void AddCountMove()
        {
            CountMove++;
        }
        
        private void Awake()
        {
            _planetPoints = new List<PlanetPoint>();
            _squads = new List<Squad>();
            _paths = new List<Path>();
            _enemySquad = null;
            _pointsInfo = Resources.LoadAll<PlanetInfo>("");
            InputClass.SpawnEnemySquad += SpawnEnemySquad;
        }

        private void Start()
        {
            if (StartMenuMediator.Instance == null)
                throw new ArgumentNullException("Instance Database not saved");
            CountMove = StartMenuMediator.Instance.CountMoveInSave;
            Deserialize();
            UpdateSettings();
        }

        private void Deserialize()
        {
            DeserializePoints();
            DeserializePaths();
            DeserializeSquads();
            DeserializeEnemySquad();
        }

        private void DeserializePoints()
        {
            Debug.Log("DeserializePoints");
            foreach (var planetInfo in _pointsInfo)
            {
                var planetPoint = _creator.CreatePlanetPoint(planetInfo);
                planetPoint.SetPlanetInfo(planetInfo);
                _planetPoints.Add(planetPoint);
            }
        }

        private void DeserializePaths()
        {
            foreach (var pointInfo in _pointsInfo)
            {
                foreach (var pathInfo in pointInfo.Paths)
                {
                    var path = _creator.CreatePath();
                    
                    var startPoint = _planetPoints.First(x => x.GetPlanetInfo() == pointInfo);
                    var endPoint = _planetPoints.First(x => x.GetPlanetInfo() == pathInfo.End);
                    
                    path.SetStartPoint(startPoint);
                    path.SetEndPoint(endPoint);

                    _paths.Add(path);
                }
            }
        }

        private void DeserializeSquads()
        {
            var squadsInfo = Resources.LoadAll<SquadInfo>("");
            var count = 0;
            foreach (var position in StartMenuMediator.Instance.PositionsInSave)
            {
                var squad = _creator.CreateSquad(position);
                var button = _creator.CreateSquadButton(squadsInfo[count++].PositionOnOrderPanel);
                squad.SetOrderButton(button);
                _squads.Add(squad);
            }
        }

        private void DeserializeEnemySquad()
        {
            var position = StartMenuMediator.Instance.EnemySquadPosition;
            if (position == Vector3.zero)
                return;
            SetEnemySquad(_creator.CreateEnemySquad(position));
        }

        private void SetEnemySquad(EnemySquad enemySquad)
        {
            _enemySquad = enemySquad;
            var position = enemySquad.transform.position;
            FindNearestPoint(position).SetEnemy(true);
            Database.SaveEnemySquadPosition(position);
            StartMenuMediator.SetEnemySquadPosition(position);
        }

        private void UpdateSettings()
        {
            UpdatePlanetPointSettings();
            UpdateSquadSettings();
        }

        private void UpdatePlanetPointSettings()
        {
            foreach (var point in _planetPoints)
            {
                point.SetPath(_paths.Where(x => x.StartPoint == point));
            }
        }

        private void UpdateSquadSettings()
        {
            foreach (var squad in _squads)
            {
                squad.Move(FindNearestPoint(squad.transform.position));
                squad.AlreadyMove = false;
            }
        }

        private void SpawnEnemySquad()
        {
            if (_enemySquad != null)
                return;
            
            var emptyPoints = _planetPoints.Where(point => point.CountSquadOnPoint == 0).ToList();
            var rnd = new Random();
            var planetPoint = emptyPoints[rnd.Next(0, emptyPoints.Count - 1)];
            var position = Vector3.zero;
            if (!planetPoint.IsDestroyed())
                position = planetPoint.transform.position + new Vector3(-IconSize, IconSize + 10f);
            if(!_creator.IsDestroyed())
                SetEnemySquad(_creator.CreateEnemySquad(position));
        }

        private PlanetPoint FindNearestPoint(Vector3 squadPosition)
        {
            PlanetPoint nearestPoint = null;
            double minDistance = double.MaxValue;
            foreach (var point in _planetPoints)
            {
                var distance = Vector3.Distance(squadPosition, point.transform.position);
                if (!(minDistance > distance)) continue;
                minDistance = distance;
                nearestPoint = point;
            }

            return nearestPoint;
        }
    }
}