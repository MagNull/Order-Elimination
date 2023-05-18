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
        public virtual void Open()
        {
            _localScale ??= transform.localScale;
            gameObject.SetActive(true);
            transform.localScale = Vector3.one * 0.1f;
            transform.DOScale(_localScale.Value, _windowOpeningTime).SetEase(_windowOpeningEase);
        }

        public virtual void Close()
        {
            transform.DOScale(0.1f, _windowOpeningTime)
                .SetEase(_windowOpeningEase)
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}