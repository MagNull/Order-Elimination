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

        public void OnMove(PlanetPoint planetPoint)
        {
            _transform.position = planetPoint.transform.position + new Vector3(-20f, 50f);
        }

        public void OnSelect()
        {
            _transform.localScale += new Vector3(0.1f, 0.1f, 0);
        }

        public void OnUnselect()
        {
            _transform.localScale -= new Vector3(0.1f, 0.1f, 0);
        }
    }
}
