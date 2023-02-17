using System;
using UIManagement;
using UnityEngine;

namespace OrderElimination
{
    public class SquadButtonTouchRace : MonoBehaviour
    {
        private float _time = 0;
        private bool _isRacePressed = false;
        public event Action Clicked;
        public event Action Holded;
        
        void Update ()
        {
            if (_isRacePressed )
            {
                _time += Time.deltaTime;
            }

            if (_time > 0.8)
            {
                Holded?.Invoke();
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
                Clicked?.Invoke();
            }

            _time = 0;
        }
    }
}
