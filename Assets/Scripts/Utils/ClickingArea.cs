using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public class ClickingArea : MonoBehaviour, IPointerDownHandler
    {
        public event Action PointerDown;
        

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke();
        }
    }
}