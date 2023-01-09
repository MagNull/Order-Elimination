using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace OrderElimination
{
    public class StrategyMap : MonoBehaviour
    {
        [SerializeField] private Creator _creator;
        [SerializeField] private Image _settingsImage;
        private PlanetInfo[] _pointsInfo;
        private List<PlanetPoint> _planetPoints;
        private List<Squad> _squads;
        private List<Path> _paths;
        public static event Action Onclick;
        public static int CountMove { get; private set; }

        public static void AddCountMove()
        {
            CountMove++;
            Database.SaveCountMove(CountMove);
        }
        
        private void Awake()
        {
            _planetPoints = new List<PlanetPoint>();
            _squads = new List<Squad>();
            _paths = new List<Path>();
            _pointsInfo = Resources.LoadAll<PlanetInfo>("");
        }

        private void Start()
        {
            if (Database.Instance == null)
                throw new ArgumentNullException("Instance Database not saved");
            CountMove = Database.Instance.CountMoveInSave;
            Deserialize();
            UpdateSettings();
        }

        private void Deserialize()
        {
            DeserializePoints();
            DeserializePaths();
            DeserializeSquads();
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
            foreach (var position in Database.Instance.PositionsInSave)
            {
                var squad = _creator.CreateSquad(position);
                var button = _creator.CreateSquadButton(squadsInfo[count++].PositionOnOrderPanel);
                squad.SetOrderButton(button);
                _squads.Add(squad);
            }
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
                squad.Move(FindNearestPoint(squad));
                squad.AlreadyMove = false;
            }
        }

        private PlanetPoint FindNearestPoint(Squad squad)
        {
            PlanetPoint nearestPoint = null;
            double minDistance = double.MaxValue;
            foreach (var point in _planetPoints)
            {
                var distance = Vector3.Distance(squad.transform.position, point.transform.position);
                if (!(minDistance > distance)) continue;
                minDistance = distance;
                nearestPoint = point;
            }

            return nearestPoint;
        }

        public void PauseButtonClicked()
        {
            _settingsImage.gameObject.SetActive(true);
        }
    }
}