using UnityEngine;

namespace OrderElimination
{
    public class PlanetPoint : MonoBehaviour
    {
        private PlanetInfo _planetInfo;
        private PlanetView _planetView;


        public PlanetPoint(Squad squad, Squad opponents)
        {
            _planetInfo = new PlanetInfo(squad, opponents);
            _planetView = new PlanetView();
        }

        public void MoveSquad(Vector2Int position) => _planetInfo.MoveSquad(position);

        public void AddOpponent(Character character) => _planetInfo.AddOpponent(character);

        public void RemoveOpponent(Character character) => _planetInfo.RemoveOpponent(character);

        public PlanetInfo GetPlanetInfo()
        {
            return _planetInfo;
        }
    }
}
