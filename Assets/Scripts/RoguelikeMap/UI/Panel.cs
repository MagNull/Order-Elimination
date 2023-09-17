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
        [SerializeField] 
        private PanelOpeningType _openingType = PanelOpeningType.None;
        private Vector3? _localScale;
        [SerializeField, ShowIf("_openingType", PanelOpeningType.Shift)]
        private float _startShiftPosition = 0f;
        [SerializeField, ShowIf("_openingType", PanelOpeningType.Shift)]
        private float _endShiftPosition = 0f;
        public bool IsOpen { get; private set; }
        
        public virtual void Open()
        {
            gameObject.SetActive(true);
            switch (_openingType)
            {
                case PanelOpeningType.Scale:
                    OpenWithScale();
                    break;
                case PanelOpeningType.Shift:
                    OpenWithShift();
                    break;
                case PanelOpeningType.None:
                default:
                    break;
            }
            IsOpen = true;
        }

        public virtual void Close()
        {
            switch (_openingType)
            {
                case PanelOpeningType.Scale:
                    CloseWithScale();
                    break;
                case PanelOpeningType.Shift:
                    CloseWithShift();
                    break;
                case PanelOpeningType.None:
                default:
                    CloseWithoutAnimation();
                    break;
            }
            IsOpen = false;
        }

        private void OpenWithScale()
        {
            _localScale ??= transform.localScale;
            gameObject.SetActive(true);
            transform.localScale = _localScale.Value * 0.1f;
            SetActiveMask(true);
            transform.DOScale(_localScale.Value, _windowOpeningTime).SetEase(_windowOpeningEase);
        }

        private void SetActiveMask(bool isActive)
        {
            if (!_isHaveMask)
                return;
            _mask.transform.SetParent(isActive ? transform.parent : transform);
            _mask.transform.localScale = Vector3.one;
            _mask.transform.SetSiblingIndex(transform.GetSiblingIndex());
            _mask.gameObject.SetActive(isActive);
            _mask.DOFade(isActive ? 0.65f : 0, _windowOpeningTime);
        }

        private void OpenWithShift()
        {
            transform.DOMoveX(_endShiftPosition, _windowOpeningTime);
        }

        private void CloseWithScale()
        {
            SetActiveMask(false);
            transform.DOScale(_localScale.Value * 0.1f, _windowOpeningTime)
                .SetEase(_windowOpeningEase)
                .OnComplete(() => gameObject.SetActive(false));
        }

        private void CloseWithShift()
        {
            transform.DOMoveX(_startShiftPosition, _windowOpeningTime);
        }

        private void CloseWithoutAnimation()
        {
            gameObject.SetActive(false);
            IsOpen = false;
        }
    }
}