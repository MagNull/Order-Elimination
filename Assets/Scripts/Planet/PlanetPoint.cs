using System.Collections.Generic;

namespace OrderElimination
{
    public class PlanetPoint
    {
        private PlanetInfo _planetInfo;
        private PlanetView _planetView;
        private List<Squad> _squads;

        public PlanetPoint()
        {
            _squads = new List<Squad>();
        }

        public PlanetInfo GetPlanetInfo() => _planetInfo;

        public void MoveSquad(Squad squad)
        {
            if(_squads.Contains(squad))
                return;
            AddSquad(squad);
            squad.Move(_planetView.GetPoisitionOnMap());
        }

        public void RemoveSquad(){ }

        private void AddSquad(Squad squad)
        {
            _squads.Add(squad);
        }
    }
}