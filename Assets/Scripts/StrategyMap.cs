using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace OrderElimination
{
    public class StrategyMap : MonoBehaviour
    {
        [SerializeField] private Creator _creator;
        [SerializeField] private Database _database;
        private PlanetInfo[] _pointsInfo;
        private List<PlanetPoint> _planetPoints;
        private List<Squad> _squads;
        private List<Path> _paths;
        public static event Action Onclick;

        private void Awake()
        {
            _planetPoints = new List<PlanetPoint>();
            _squads = new List<Squad>();
            _paths = new List<Path>();
            _pointsInfo = Resources.LoadAll<PlanetInfo>("");
        }

        private async void Start()
        {
            await Deserialize();
            UpdateSettings();
        }

        private async Task Deserialize()
        {
            DeserializePoints();
            DeserializePaths();
            await DeserializeSquads();
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
                var startPosition = pointInfo.Position;
                foreach (var pathInfo in pointInfo.Paths)
                {
                    var endPosition = pathInfo.End.Position;
                    
                    var pathPosition = GetPathPosition(startPosition, endPosition);
                    var quaternion = GetPathQuaternion(startPosition, endPosition);
                    var path = _creator.CreatePath(pathPosition, quaternion);
                    //scale for distance
                    path.transform.localScale = new Vector3(Vector3.Distance(startPosition, endPosition) * 0.075f, 60, 1);
                    path.SetStartPoint(_planetPoints.FirstOrDefault(x => x.GetPlanetInfo() == pointInfo));
                    path.SetEndPoint(_planetPoints.FirstOrDefault(x => x.GetPlanetInfo() == pathInfo.End));
                    _paths.Add(path);
                }
            }
        }

        private Vector3 GetPathPosition(Vector3 start, Vector3 end)
        {
            var pathPositionX = (end.x + start.x) / 2;
            var pathPositionY = end.y - 25;
            if (start.y == end.y) 
                return new Vector3(pathPositionX, pathPositionY);

            pathPositionX += 25;
            pathPositionY = (end.y + start.y) / 2 - 25;
            if(end.y < start.y)
                pathPositionX -= 50;
            return new Vector3(pathPositionX, pathPositionY);
        }

        private Quaternion GetPathQuaternion(Vector3 start, Vector3 end)
        {
            if(Math.Abs(start.y - end.y) < 1)
                return Quaternion.identity;

            var b = end.y - start.y;
            var a = end.x - start.x;
            var alpha = Math.Atan(b / a) * 180 / Math.PI;
            
            return start.x < end.x 
                ? Quaternion.Euler(0, 0, (float)alpha)
                : Quaternion.Euler(0, 0, -(float)alpha);
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
            for (var i = 0; i < _paths.Count; i++)
            {
                _paths[i].gameObject.SetActive(false);
            }
        }

        private void UpdatePlanetPointSettings()
        {
            for (var i = 0; i < _pointsInfo.Length; i++)
            {
                _planetPoints[i].SetPath(_paths.Where(x => x.Start == _planetPoints[i]));
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