using UIManagement.Elements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoguelikeMap.UI.Characters
{
    public class DraggableObject : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private HoldableButton _button;

        private Transform _initialParent;
        private Transform _defaultParent;
        private CanvasGroup _canvasGroup;
        
        private void Start()
        {
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            _button.Holded += OnBeginDrag;
        }
        
        private void OnBeginDrag(HoldableButton button, float holdTimeInSecond)
        {
            Debug.Log("OnBeginDrag");
            _initialParent = transform.parent;
            _defaultParent = _initialParent.parent;
            transform.SetParent(_defaultParent);
            _canvasGroup.blocksRaycasts = false;
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
    }
}