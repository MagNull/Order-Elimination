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
        [SerializeField]
        private Creator _creator;
        private PlanetInfo[] _pointsInfo;
        private List<PlanetPoint> _planetPoints;
        private List<Squad> _squads;
        private List<Path> _paths;
        private EnemySquad _enemySquad;
        public const float IconSize = 50f;
        public static event Action Onclick;
        public static int SaveIndex { get; private set; }
        public static int CountMove { get; private set; }

        public IReadOnlyList<Squad> Squads => _squads;

        public IReadOnlyList<PlanetPoint> PlanetPoints => _planetPoints;

        public EnemySquad EnemySquad => _enemySquad;

        public static void AddCountMove()
        {
            CountMove++;
        }

        //VERY VERY COOL CRUTCH TODO: Fix
        public void SpawnEnemy(int index)
        {
            var emptyPoints = _planetPoints.Where(point => point.CountSquadOnPoint == 0).ToList();
            var planetPoint = emptyPoints[index];
            var position = planetPoint.transform.position + new Vector3(-IconSize, IconSize + 10f);
            SetEnemySquad(_creator.CreateEnemySquad(position));
        }

        private void Awake()
        {
            _planetPoints = new List<PlanetPoint>();
            _squads = new List<Squad>();
            _paths = new List<Path>();
            _enemySquad = null;
            _pointsInfo = Resources.LoadAll<PlanetInfo>("");
            InputClass.onFinishMove += OnFinishMove;
        }

        private void Start()
        {
            SaveIndex = PlayerPrefs.GetInt("SaveIndex");
            CountMove = PlayerPrefs.GetInt($"{SaveIndex}:CountMove");
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
            var positionsInSave = new List<Vector3>
            {
                PlayerPrefs.GetString($"{SaveIndex}:Squad 0").GetVectorFromString(),
                PlayerPrefs.GetString($"{SaveIndex}:Squad 1").GetVectorFromString()
            };
            foreach (var position in positionsInSave)
            {
                var squad = _creator.CreateSquad(position, count == 0);
                squad.name = $"Squad {count}";
                var button = _creator.CreateSquadButton(squadsInfo[count].PositionOnOrderPanel, count == 0);
                count++;
                squad.SetOrderButton(button);
                _squads.Add(squad);
            }
        }

        private void DeserializeEnemySquad()
        {
            var position = PlayerPrefs.GetString($"{SaveIndex}:EnemySquad").GetVectorFromString();
            if (position == Vector3.zero)
                return;
            var battleOutcome = PlayerPrefs.GetString($"{SaveIndex}:BattleOutcome");
            if (battleOutcome != BattleOutcome.Victory.ToString())
            {
                SetEnemySquad(_creator.CreateEnemySquad(position));
            }
            else
                Database.DeleteEnemySquadPosition();
        }

        private void SetEnemySquad(EnemySquad enemySquad)
        {
            _enemySquad = enemySquad;
            var position = enemySquad.transform.position;
            FindNearestPoint(position).SetEnemy(true);
            Database.SaveEnemySquadPosition(position);
            PlayerPrefs.SetString($"{SaveIndex}:EnemySquad", position.ToString());
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
            var count = 0;
            foreach (var squad in _squads)
            {
                squad.Move(FindNearestPoint(squad.transform.position));
                squad.SetAlreadyMove(PlayerPrefs.GetInt($"{SaveIndex}:Squad {count}:isMove") == 1);
                //squad.AlreadyMove = StartMenuMediator.Instance.IsMoveSquads[count++];
            }
        }

        private void OnFinishMove()
        {
            if (_enemySquad != null)
                return;

            var emptyPoints = _planetPoints.Where(point => point.CountSquadOnPoint == 0).ToList();
            var rnd = new Random();
            var planetPoint = emptyPoints[rnd.Next(0, emptyPoints.Count - 1)];
            var position = Vector3.zero;
            if (!planetPoint.IsDestroyed())
                position = planetPoint.transform.position + new Vector3(-IconSize, IconSize + 10f);
            if (!_creator.IsDestroyed())
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

        private void OnDisable()
        {
            InputClass.onFinishMove -= OnFinishMove;
        }
    }
}