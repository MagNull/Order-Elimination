using UnityEngine;


namespace OrderElimination
{
    public class PlanetView
    {
        private Transform _transform;

        public PlanetView(Transform transform)
        {
            _transform = transform;
        }

        public void Increase()
        {
            _transform.localScale += new Vector3(0.1f, 0.1f, 0);
        }

        public void Decrease()
        {
            _transform.localScale -= new Vector3(0.1f, 0.1f, 0);
        }
    }
}