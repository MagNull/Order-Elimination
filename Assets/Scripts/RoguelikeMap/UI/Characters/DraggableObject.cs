using UnityEngine;
using UnityEngine.EventSystems;

namespace RoguelikeMap.UI.Characters
{
    public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Transform _initialParent;
        private Transform _defaultParent;
        private CanvasGroup _canvasGroup;
        
        private void Start()
        {
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("OnEndDrag");
            if(transform.parent == _defaultParent)
                transform.SetParent(_initialParent);
            _canvasGroup.blocksRaycasts = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("OnBeginDrag");
            _initialParent = transform.parent;
            _defaultParent = _initialParent.parent;
            transform.SetParent(_defaultParent);
            _canvasGroup.blocksRaycasts = false;
        }
    }
}