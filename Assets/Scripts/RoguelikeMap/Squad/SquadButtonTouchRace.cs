using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OrderElimination
{
    public class SquadButtonTouchRace : MonoBehaviour
    {
        private int MillisecondsToHoldError => Math.Min(10, MillisecondsToHold);
        [SerializeField]
        public int MillisecondsToHold = 700;
        private float _time = 0;
        private bool _isRacePressed = false;
        public event Action Clicked;
        public event Action Holded;
        
        private void OnMouseDown()
        {
            _isRacePressed = true;
            _time = Time.unscaledTime;
            UniTask.Create(WaitUntilHoldTime);
        }
        
        private async UniTask WaitUntilHoldTime()
        {
            await UniTask.Delay(MillisecondsToHold, ignoreTimeScale: true);
            if (!_isRacePressed)
                return;
            var holdTime = Time.unscaledTime - _time;
            if (holdTime * 1000 + MillisecondsToHoldError >= MillisecondsToHold)
            {
                Holded?.Invoke();
            }
        }

        private void OnMouseUp()
        {
            _isRacePressed = false;
            var holdTime = Time.unscaledTime - _time;
            if (holdTime * 1000 + MillisecondsToHoldError < MillisecondsToHold)
            {
                Clicked?.Invoke();
            }

            _time = 0;
        }
    }
}
