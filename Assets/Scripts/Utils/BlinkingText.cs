using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Utils
{
    public class BlinkingText : MonoBehaviour
    {
        [SerializeField] 
        private float _interval = 1f;
        [SerializeField]
        private TMP_Text _text;
        
        private bool _isVisible = true;
        private float _endValue = 0;
        private float _startValue = 1;
        
        private void Start()
        {
            InvokeRepeating("ToggleVisibility", 0, _interval);
        }

        private void ToggleVisibility()
        {
            _isVisible = !_isVisible;
            _text.DOFade(_isVisible ? _endValue : _startValue, _interval);
        }
    }
}