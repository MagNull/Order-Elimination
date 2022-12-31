using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace OrderElimination
{
    public class StrategyMap : MonoBehaviour
    {
        [SerializeField] private Creator _creator;
        [SerializeField] private Database _database;
        private List<PlanetPoint> _planetPoints;
        private List<Squad> _squads;
        private List<Path> _paths;
        public static event Action Onclick;

        private void Awake()
        {
            _planetPoints = new List<PlanetPoint>();
            _squads = new List<Squad>();
            _paths = new List<Path>();
        }

        private async void Start()
        {
            await Deserialize();
            UpdateSettings();
        }

        private async Task Deserialize()
        {
            DeserializePaths();
            DeserializePoints();
            await DeserializeSquads();
        }

        private void DeserializePaths()
        {
            var pathsInfo = Resources.LoadAll<PathInfo>("");
            foreach (var a in pathsInfo)
                _paths.Add(_creator.CreatePath(a));
        }

        private void DeserializePoints()
        {
            var pointsInfo = Resources.LoadAll<PlanetInfo>("");
            foreach (var a in pointsInfo)
                _planetPoints.Add(_creator.CreatePlanetPoint(a));
        }

        private async Task DeserializeSquads()
        {
            var positions = _database.LoadData();
            var squadsInfo = Resources.LoadAll<SquadInfo>("");
            var count = 0;
            await foreach(var position in positions)
            {
                var squad = _creator.CreateSquad(GetVectorFromString(position));
                var button = _creator.CreateSquadButton(squadsInfo[count++].PositionOnOrderPanel);
                squad.SetOrderButton(button);
                _squads.Add(squad);
            }
        }

        private Vector3 GetVectorFromString(string str)
        {
            var temp = str.Split(new[] { "(", ")", ", ", ".00" }, StringSplitOptions.RemoveEmptyEntries);
            
            var x = Convert.ToInt32(temp[0]);
            var y = Convert.ToInt32(temp[1]);
            var z = Convert.ToInt32(temp[2]);
            return new Vector3(x, y, z);
        }

        private void UpdateSettings()
        {
            UpdatePathSettings();
            UpdatePlanetPointSettings();
            UpdateSquadSettings();
        }

        private void UpdatePathSettings()
        {
            for (int i = 0; i < _paths.Count; i++)
            {
                _paths[i].SetStartPoint(_planetPoints[i]);
                _paths[i].SetEndPoint(_planetPoints[i + 1]);
                _paths[i].gameObject.SetActive(false);
            }
        }

        private void UpdatePlanetPointSettings()
        {
            for (int i = 0; i < _planetPoints.Count; i++)
            {
                if (i != _planetPoints.Count - 1)
                    _planetPoints[i].SetPath(_paths[i]);
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
    }
}