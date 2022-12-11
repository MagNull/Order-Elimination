using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using UnityEngine.EventSystems;

namespace OrderElimination
{
    public class StrategyMap : MonoBehaviour
    {
        [SerializeField] private Creator _creator;
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

        private void Start()
        {
            Deserialize();
            UpdateSettings();
        }

        private void Deserialize()
        {
            DeserializePaths();
            DeserializePoints();
            DeserializeSquads();
        }


        private void DeserializePaths()
        {
            var pathsInfo = Resources.LoadAll<PathInfo>("");
            foreach(var a in pathsInfo)
                _paths.Add(_creator.CreatePath(a));
        }

        private void DeserializePoints()
        {
            var pointsInfo = Resources.LoadAll<PlanetInfo>("");
            foreach(var a in pointsInfo)
                _planetPoints.Add(_creator.CreatePlanetPoint(a));
        }

        private void DeserializeSquads()
        {
            var positions = GetPositionsSquad();
            
            var squadsInfo = Resources.LoadAll<SquadInfo>("");
            var count = 0;
            foreach(var a in squadsInfo)
            {
                var squad = _creator.CreateSquad(positions[count++]);
                var button = _creator.CreateSquadButton(a.PositionOnOrderPanel);
                squad.SetOrderButton(button);
                _squads.Add(squad);
            }
        }

        //Не заглядывай сюда если жизнь дорога
        private List<Vector3> GetPositionsSquad()
        {
            using var reader = XmlReader
                .Create(Application.dataPath + "/Resources" + "/Xml" + "/SquadPositions.xml");
            reader.MoveToContent();
            var positionsString = reader.ReadElementContentAsString();
            
            var temp = positionsString
                .Split(new []{"(", ")", ", ", ".00"}, StringSplitOptions.RemoveEmptyEntries);
            var positions = new List<Vector3>();
            for (int i = 0; i < temp.Length; i += 3)
            {
                var x = Convert.ToInt32(temp[i]);
                var y = Convert.ToInt32(temp[i + 1]);
                var z = Convert.ToInt32(temp[i + 2]);
                positions.Add(new Vector3(x, y, z));
            }

            return positions;
        }
        
        private void UpdateSettings()
        {
            UpdatePathSettings();
            UpdatePlanetPointSettings();
            UpdateSquadSettings();
        }

        private void UpdatePathSettings()
        {
            Debug.Log(_paths.Count);
            for(int i = 0; i < _paths.Count; i++)
            {
                _paths[i].SetStartPoint(_planetPoints[i]);
                _paths[i].SetEndPoint(_planetPoints[i + 1]);
                _paths[i].gameObject.SetActive(false);
            }
        }

        private void UpdatePlanetPointSettings()
        {
            for(int i = 0; i < _planetPoints.Count; i++)
            {
                if(i != _planetPoints.Count - 1)
                    _planetPoints[i].SetPath(_paths[i]);
            }
        }

        private void UpdateSquadSettings()
        {
            foreach(var squad in _squads)
            {
                squad.Move(FindNearestPoint(squad));
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