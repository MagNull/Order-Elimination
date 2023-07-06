using Maths;
using OrderElimination;
using UnityEngine;

namespace DefaultNamespace
{
    public class TestSolver: MonoBehaviour
    {
        
        [SerializeField] private Vector2 _startPoint = new Vector2(0, 0);
        [SerializeField] private Vector2 _endPoint = new Vector2(3, 3);
        void Start()
        {
            foreach (var v in IntersectionSolver.GetIntersections(_startPoint, _endPoint))
            {
                Logging.Log($"{v.CellPosition}: {v.SmallestPartSquare}, {v.IntersectionAngle}");
            }
        }
    }
}