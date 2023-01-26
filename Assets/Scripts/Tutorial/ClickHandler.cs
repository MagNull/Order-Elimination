using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tutorial
{
    public class ClickHandler : MonoBehaviour, IPointerDownHandler
    {
        public event Action Clicked;
        public void OnPointerDown(PointerEventData eventData)
        {
            OnClicked();
        }

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            OnClicked();
        }

        private void OnClicked()
        {
            Debug.Log("Clicked");
            Clicked?.Invoke();
            Destroy(this);
        }
    }
}