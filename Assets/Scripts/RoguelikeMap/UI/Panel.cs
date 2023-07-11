using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap.UI
{
    public class Panel : MonoBehaviour
    {
        [SerializeField]
        private float _windowOpeningTime = 0.3f;
        [SerializeField]
        private Ease _windowOpeningEase = Ease.Flash;
        [SerializeField]
        private bool _isHaveMask;
        [SerializeField, ShowIf("_isHaveMask")]
        private Image _mask;
        private Vector3? _localScale;
        public bool IsOpen { get; private set; }
        
        public virtual void Open()
        {
            _localScale ??= transform.localScale;
            gameObject.SetActive(true);
            transform.localScale = _localScale.Value * 0.1f;
            SetActiveMask(true);
            transform.DOScale(_localScale.Value, _windowOpeningTime).SetEase(_windowOpeningEase);
            IsOpen = true;
        }

        public virtual void Close()
        {
            SetActiveMask(false);
            transform.DOScale(_localScale.Value * 0.1f, _windowOpeningTime)
                .SetEase(_windowOpeningEase)
                .OnComplete(() => gameObject.SetActive(false));
            IsOpen = false;
        }

        private void SetActiveMask(bool isActive)
        {
            if (!_isHaveMask)
                return;
            _mask.transform.SetParent(isActive ? transform.parent : transform);
            _mask.transform.localScale = Vector3.one;
            _mask.transform.SetSiblingIndex(transform.GetSiblingIndex());
            _mask.gameObject.SetActive(isActive);
        }

        public void OpenWithoutAnimation()
        {
            gameObject.SetActive(true);
            IsOpen = true;
        }

        public void CloseWithoutAnimation()
        {
            gameObject.SetActive(false);
            IsOpen = false;
        }
    }
}