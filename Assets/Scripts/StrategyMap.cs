using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;

namespace OrderElimination
{
    public class StrategyMap : MonoBehaviour
    {
        [SerializeField] private Creator _creator;
        [SerializeField] private SelectableObjects _selectableObjects;
        private List<PlanetPoint> _planetPoints;
        private List<Squad> _squads;
        private List<Path> _paths;
        private List<ISelectable> _selectedObjects;

        private void Awake()
        {
            _planetPoints = new List<PlanetPoint>();
            _squads = new List<Squad>();
            _paths = new List<Path>();
            _selectedObjects = new List<ISelectable>();
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
            {
                _paths.Add(_creator.CreatePath(a));
            }
        }

        private void DeserializePoints()
        {
            var pointsInfo = Resources.LoadAll<PlanetInfo>("");
            foreach(var a in pointsInfo)
            {
                _planetPoints.Add(_creator.CreatePlanetPoint(a));
            }
        }

        private void DeserializeSquads()
        {
            var squadsInfo = Resources.LoadAll<SquadInfo>("");
            foreach(var a in squadsInfo)
            {
                _squads.Add(_creator.CreateSquad(a));
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
                
                _planetPoints[i].Selected += AddSelectedObjects;
            }
        }

        private void UpdateSquadSettings()
        {
            foreach(var squad in _squads)
            {
                squad.SetPlanetPoint(_planetPoints[0]);
                squad.Selected += AddSelectedObjects;;
            }
        }

        private void AddSelectedObjects(ISelectable selectedObject)
        {
            if(_selectedObjects.Count == 2)
                _selectedObjects.Clear();
            _selectedObjects.Add(selectedObject);
        }
    }
}