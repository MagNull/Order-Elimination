using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
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

        private float _lastEmitTime = 0;
        
        public async void Emit(string text, Color color)
        {
            await UniTask.WaitUntil(() => Time.time - _lastEmitTime >= _delayBetweenSpawn);
            _lastEmitTime = Time.time;
            var textInstance = Instantiate(_textPrefab, transform);
            textInstance.text = text;
            textInstance.color = color;
            textInstance.transform.DOMoveY(transform.position.y + _speed * _duration, _duration);
            textInstance.DOFade(0, _duration).OnComplete(() => Destroy(textInstance.gameObject));
        }
    }
}