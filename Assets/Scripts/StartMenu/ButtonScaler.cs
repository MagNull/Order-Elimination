using UnityEngine;
using UnityEngine.EventSystems;

namespace OrderElimination
{
    public class ButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float _exitScale = 1.0f;
        [SerializeField] private float _enterScale = 1.1f;

        private void OnDisable()
        {
            gameObject.transform.localScale = new Vector3(_exitScale, _exitScale, _exitScale);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            gameObject.transform.localScale = new Vector3(_enterScale, _enterScale, _enterScale);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            gameObject.transform.localScale = new Vector3(_exitScale, _exitScale, _exitScale);
        }
    }
}

