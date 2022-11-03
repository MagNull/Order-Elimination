using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

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
            DeserializePaths();
            //DeserializePoints();
            //DeserializeSquads();
        }

        private void DeserializePaths()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Vector3>));
            List<Vector3> cordinates;
            using (FileStream fs = new FileStream(Application.dataPath + "/Paths.xml", FileMode.OpenOrCreate))
            {
                cordinates = serializer.Deserialize(fs) as List<Vector3>;
            }
            foreach(var a in cordinates)
            {
                _paths.Add(_creator.CreatePath(a));
                Debug.Log(_paths.Count);
                Debug.Log(_paths[_paths.Count - 1].gameObject.transform.position.x);
            }
        }

        private void DeserializePoints()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Vector3>));
            List<Vector3> cordinates;
            using (FileStream fs = new FileStream(Application.dataPath + "/Points.xml", FileMode.OpenOrCreate))
            {
                cordinates = serializer.Deserialize(fs) as List<Vector3>;
            }
        }

        private void DeserializeSquads()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Vector3>));
            List<Vector3> cordinates;
            using (FileStream fs = new FileStream(Application.dataPath + "/Squads.xml", FileMode.OpenOrCreate))
            {
                cordinates = serializer.Deserialize(fs) as List<Vector3>;
            }
        }
    }
}