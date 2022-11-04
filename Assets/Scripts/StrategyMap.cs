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
            XmlSerializer serializer = new XmlSerializer(typeof(List<Vector3>));
            List<Vector3> positiones;
            using (FileStream fs = new FileStream(Application.dataPath + "/Paths.xml", FileMode.OpenOrCreate))
            {
                positiones = serializer.Deserialize(fs) as List<Vector3>;
            }
            foreach(var a in positiones)
            {
                _paths.Add(_creator.CreatePath(a));
            }
        }

        private void DeserializePoints()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Vector3>));
            List<Vector3> positiones;
            using (FileStream fs = new FileStream(Application.dataPath + "/Points.xml", FileMode.OpenOrCreate))
            {
                positiones = serializer.Deserialize(fs) as List<Vector3>;
            }
            foreach(var a in positiones)
            {
                _planetPoints.Add(_creator.CreatePlanetPoint(a));
            }
        }

        private void DeserializeSquads()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Vector3>));
            List<Vector3> positiones;
            using (FileStream fs = new FileStream(Application.dataPath + "/Squads.xml", FileMode.OpenOrCreate))
            {
                positiones = serializer.Deserialize(fs) as List<Vector3>;
            }
            foreach(var a in positiones)
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