using System;
using Cinemachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap.UI
{
    public class Panel : SerializedMonoBehaviour
    {
        [SerializeField]
        protected float _windowOpeningTime = 0.3f;
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
        protected bool _isHaveCameraShift;
        [SerializeField, ShowIf("_isHaveCameraShift")]
        private CinemachineVirtualCamera _camera;
        [SerializeField, ShowIf("_openingType", PanelOpeningType.Shift)]
        private CanvasScaler _canvasScaler;
        [SerializeField, ShowIf("_openingType", PanelOpeningType.Shift)]
        protected float _shift;

        protected float _scaleRatio;
        private CinemachineTransposer _transposer;

        public event Action OnOpen;
        public event Action OnClose;
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
            OnOpen?.Invoke();
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
            OnClose?.Invoke();
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

        protected virtual void OpenWithShift()
        {
            if(_scaleRatio is default(float))
                InitializeCanvasSettings();

            transform.DOMoveX(Screen.width - _shift * _scaleRatio, _windowOpeningTime);
            if(_isHaveCameraShift)
                DoCameraShift();
        }

        protected void DoCameraShift()
        {
            _transposer ??= _camera.GetCinemachineComponent<CinemachineTransposer>();
            
            DOTween.To(() => _transposer.m_FollowOffset.x, 
                    x => _transposer.m_FollowOffset.x = x, IsOpen ? 400 : 800,
                    _windowOpeningTime)
                .SetEase(_windowOpeningEase);
        }

        private void CloseWithScale()
        {
            SetActiveMask(false);
            transform.DOScale(_localScale.Value * 0.1f, _windowOpeningTime)
                .SetEase(_windowOpeningEase)
                .OnComplete(() => gameObject.SetActive(false));
        }

        protected virtual void CloseWithShift()
        {
            if(_scaleRatio is default(float))
                InitializeCanvasSettings();
            
            transform.DOMoveX(Screen.width + _shift * _scaleRatio, _windowOpeningTime);
            if(_isHaveCameraShift)
                DoCameraShift();
        }
        
        protected void InitializeCanvasSettings()
        {
            if (_openingType is not PanelOpeningType.Shift)
                return;
            float canvasWidth = _canvasScaler.referenceResolution.x;
            float canvasHeight = _canvasScaler.referenceResolution.y;

            _scaleRatio = _canvasScaler.matchWidthOrHeight > 0.5f 
                ? Screen.width / canvasWidth 
                : Screen.height / canvasHeight;
        }

        protected void CloseWithoutAnimation()
        {
            gameObject.SetActive(false);
            IsOpen = false;
        }
    }
}