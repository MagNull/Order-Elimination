using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RoguelikeMap
{
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;

        [SerializeField]
        private TMP_Text _text;

        public event Action<int> OnVolumeChanged;

        private void Awake()
        {
            _slider.onValueChanged.AddListener(VolumeChanged);
        }

        public void Initialize(int volume)
        {
            _text.text = volume + "%";
            _slider.value = volume / 100f;
        }

        private void VolumeChanged(float value)
        {
            var value_int = Convert.ToInt32(value * 100);
            _text.text = (value_int).ToString() + "%";
            OnVolumeChanged?.Invoke(value_int);
        }
    }
}