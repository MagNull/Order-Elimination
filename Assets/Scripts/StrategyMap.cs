using System.Collections.Generic;
using UnityEngine;
using System;
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
            var squadsInfo = Resources.LoadAll<SquadInfo>("");
            foreach(var a in squadsInfo)
            {
                var squad = _creator.CreateSquad(a);
                var button = _creator.CreateSquadButton(a.PositionOnOrderPanel);
                squad.SetOrderButton(button);
                _squads.Add(squad);
            }
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
                //TODO
                //Пока задается как первый поинт. Если дальше будет сохранение - поменять
                _planetPoints[0].MoveSquad(squad);
            }
        }

        public void OnMouseUp() 
        {
            Onclick?.Invoke();
        }
    }
}