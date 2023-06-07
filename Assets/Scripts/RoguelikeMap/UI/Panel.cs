using DG.Tweening;
using UnityEngine;

namespace RoguelikeMap.UI
{
    public class Panel : MonoBehaviour
    {
        [SerializeField]
        private float _windowOpeningTime = 0.3f;
        [SerializeField]
        private Ease _windowOpeningEase = Ease.Flash;
        
        private Vector3? _localScale;
        
        public bool IsOpen { get; private set; }
        
        public virtual void Open()
        {
            _localScale ??= transform.localScale;
            gameObject.SetActive(true);
            transform.localScale = _localScale.Value * 0.1f;
            transform.DOScale(_localScale.Value, _windowOpeningTime).SetEase(_windowOpeningEase);
            IsOpen = true;
        }

        public virtual void Close()
        {
            transform.DOScale(_localScale.Value * 0.1f, _windowOpeningTime)
                .SetEase(_windowOpeningEase)
                .OnComplete(() => gameObject.SetActive(false));
            IsOpen = false;
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