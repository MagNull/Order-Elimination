using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    //TODO-OPTIMIZE: implement ObjectPool
    public class TextEmitter : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textPrefab;
        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _duration;
        [SerializeField]
        private float _delayBetweenSpawn;
        private float _standardFontSize;
        private float _lastEmitTime = 0;

        public float StandardFontSize => _standardFontSize;

        private void Start()
        {
            _standardFontSize = _textPrefab.fontSize;
        }

        public async UniTask Emit(string text, Color color, float fontSize = -1)
        {
            await Emit(text, color, transform.position, _speed * _duration, fontSize);
        }

        public async UniTask Emit(string text, Color color, Vector3 position, float fontSize = -1)
        {
            await Emit(text, color, position, _speed * _duration, fontSize);
        }

        public async UniTask Emit(string text, Color color, Vector3 position, float offsetY = 0, float duration = 1, float fontSize = -1)
        {
            await Emit(text, color, position, new Vector3(0, offsetY, 0), duration, fontSize);
        }

        public async UniTask Emit(string text, Color color, Vector3 position, Vector3 offset, float fontSize = -1)
        {
            await Emit(text, color, position, offset, _duration, fontSize);
        }

        public async UniTask Emit(string text, Color color, Vector3 position, Vector3 offset, float duration, float fontSize = -1)
        {
            await Emit(text, color, position, offset, 1, duration, fontSize);
        }

        public async UniTask Emit(string text, Color color, Vector3 position, Vector3 offset, float scaleOffset, float duration, float fontSize = -1)
        {
            await UniTask.WaitUntil(() => Time.time - _lastEmitTime >= _delayBetweenSpawn);
            _lastEmitTime = Time.time;
            var textInstance = Instantiate(_textPrefab, transform);
            textInstance.transform.position = position;
            textInstance.gameObject.SetActive(true);
            textInstance.fontSize = fontSize > 0 ? fontSize : _standardFontSize;
            textInstance.text = text;
            textInstance.color = color;
            textInstance.DOComplete();
            textInstance.transform.DOComplete();
            textInstance.transform.DOMove(position + offset, duration);
            var endScale = textInstance.transform.localScale * scaleOffset;
            textInstance.transform.DOScale(endScale, duration);
            textInstance.DOFade(0, duration).SetEase(Ease.InFlash).OnComplete(() => Destroy(textInstance.gameObject));
        }
    }
}