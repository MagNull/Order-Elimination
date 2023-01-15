using System;
using UIManagement;
using UnityEngine;

namespace OrderElimination
{
    public class SquadButtonTouchRace : MonoBehaviour
    {
        private float _time = 0;
        private bool _isRacePressed = false;
        public event Action onPointerDown;
        public event Action onActiveSquadPanel;
        
        void Update ()
        {
            if (_isRacePressed )
            {
                _time += Time.deltaTime;
                Debug.Log(_time);
            }

            if (_time > 0.8)
            {
                onActiveSquadPanel?.Invoke();
                PointerUp();
            }
        }
        
        public void PointerDown()
        {
            _isRacePressed = true;
        }
        
        public void PointerUp()
        {
            _isRacePressed = false;
            if (_time < 0.5)
            {
                onPointerDown?.Invoke();
            }

            _time = 0;
        }
    }
}
