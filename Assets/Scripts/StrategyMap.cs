using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

namespace OrderElimination
{
    public class StrategyMap : MonoBehaviour
    {
        [SerializeField] private Creator _creator;

        private List<PlanetPoint> _planetPoints;
        private List<Squad> _squads;
        private List<Path> _paths;

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
            UpdateSquadSettings();
        }

        private void UpdateSquadSettings()
        {
            foreach(var squad in _squads)
                squad.SetPlanetPoint(_planetPoints[0]);
        }
    }
}