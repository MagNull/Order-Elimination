using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.Start
{
    public class StartMenuMediator : MonoBehaviour
    {
        private List<Vector3> _positionsInSave;
        public int CountMoveInSave { get; private set; }
        public IReadOnlyList<Vector3> PositionsInSave => _positionsInSave;
        public static StartMenuMediator Instance;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public static void SetPositionsInSave(List<Vector3> positions)
        {
            Instance._positionsInSave = positions;
        }

        public static void SetCountMove(int countMove)
        {
            Instance.CountMoveInSave = countMove;
        }
    }
}