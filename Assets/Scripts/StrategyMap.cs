using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
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
        private const float IconSize = 50;
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
                    var quaternion = GetPathQuaternion(startPosition, endPosition);
                    var pathPosition = GetPathPosition(startPosition, endPosition, quaternion);
                    
                    var path = _creator.CreatePath(pathPosition, quaternion);
                    //scale for distance
                    path.transform.localScale =
                        new Vector3(Vector3.Distance(startPosition, endPosition) * 0.075f, 60, 1);

                    var startPoint = _planetPoints.First(x => x.GetPlanetInfo() == pointInfo);
                    var endPoint = _planetPoints.First(x => x.GetPlanetInfo() == pathInfo.End);
                    
                    path.SetStartPoint(startPoint);
                    path.SetEndPoint(endPoint);

                    _paths.Add(path);
                }
            }
        }

        private Vector3 GetPathPosition(Vector3 start, Vector3 end, Quaternion quaternion)
        {
            var pathPositionX = (end.x + start.x) / 2;
            var pathPositionY = (end.y + start.y) / 2;
            if (Math.Abs(start.x - end.x) < 0.5)
                return new Vector3(pathPositionX + IconSize / 2, pathPositionY);
            if (Math.Abs(start.y - end.y) < 0.5)
                return new Vector3(pathPositionX, pathPositionY - IconSize / 2);
            
            if (end.y > start.y && end.x > start.x)
                pathPositionX += IconSize / 2;

            if (start.y > end.y && start.x > end.x)
                pathPositionX += IconSize / 2;

            if (quaternion.z < 0)
                pathPositionX -= IconSize / 2;
            return new Vector3(pathPositionX, pathPositionY);
        }

        private Quaternion GetPathQuaternion(Vector3 start, Vector3 end)
        {
            if (Math.Abs(start.y - end.y) < 1)
                return Quaternion.identity;
            if (Math.Abs(start.x - end.x) < 1)
                return Quaternion.Euler(0, 0, 90);
            
            var b = end.y - start.y;
            var a = end.x - start.x;
            var alpha = MathF.Atan(b / a) * 180 / (float)Math.PI;

            return Quaternion.Euler(0, 0, alpha);
        }

        private async Task DeserializeSquads()
        {
            var positions = _database.LoadData();
            var squadsInfo = Resources.LoadAll<SquadInfo>("");
            var count = 0;
            await foreach (var position in positions)
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
            UpdatePlanetPointSettings();
            UpdateSquadSettings();
        }

        private void UpdatePlanetPointSettings()
        {
            foreach (var point in _planetPoints)
            {
                point.SetPath(_paths.Where(x => x.Start == point));
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