using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        public Slider _slider;
        
        public void SetMaxHealth(int health)
        {
            _slider.maxValue = health;
            _slider.value = health;
        }
    
        public void SetHealth(int health)
        {
            _slider.value = health;
        }
    }
}