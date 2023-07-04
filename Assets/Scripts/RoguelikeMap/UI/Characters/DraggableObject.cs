using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoguelikeMap.UI.Characters
{
    public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private bool _isTransparency;
        [SerializeField, ShowIf("_isTransparency")]
        private Image _image;
        [SerializeField, Range(0, 1), ShowIf("_isTransparency")] 
        private float _aplha;
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
            if (transform.parent == _defaultParent)
            {
                transform.SetParent(_initialParent);
                transform.localPosition = Vector3.zero;
            }

            if (_isTransparency)
                ChangeTransparency();
            _canvasGroup.blocksRaycasts = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _initialParent = transform.parent;
            _defaultParent = _initialParent.parent.parent;
            transform.SetParent(_defaultParent);
            if(_isTransparency)
                ChangeTransparency(_aplha);
            _canvasGroup.blocksRaycasts = false;
        }

        private void ChangeTransparency(float alpha = 1)
        {
            if(_image is null)
                Debug.Log("Image is null in DraggableObject");
            var color = _image.color;
            color.a = alpha;
            _image.color = color;
        }
    }
}