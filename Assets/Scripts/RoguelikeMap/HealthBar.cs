using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;
           
        public void SetHealth(float maxHealth, float currentHealth)
        {
            _slider.maxValue = maxHealth;
            _slider.value = currentHealth;
        }
    }
}