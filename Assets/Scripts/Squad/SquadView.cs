using UnityEngine;

namespace OrderElimination
{
    public class SquadView
    {
        private Transform _transform;

        public SquadView(Transform transform)
        {
            _transform = transform;
        }

        //TODO(Иван): Magic numbers
        public void OnMove(PlanetPoint planetPoint)
        {
            _transform.position = planetPoint.transform.position +
                                  new Vector3(-50 + (planetPoint.CountSquadOnPoint - 1) * 100f, 60f);
        }
    }
}