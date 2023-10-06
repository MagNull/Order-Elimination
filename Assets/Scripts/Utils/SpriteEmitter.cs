using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace OrderElimination.Utils
{
    public class SpriteEmitter : MonoBehaviour
    {
        private ObjectPool<SpriteRenderer> _spritePool;

        [SerializeField]
        private SpriteRenderer _spritePrefab;

        [SerializeField]
        private float _defaultSpriteSize = 1f;

        private void Awake()
        {
            _spritePool = new(
                () => Instantiate(_spritePrefab, transform),
                OnGet,
                OnRelease);
        }

        public async UniTask Emit(Sprite sprite, Vector3 position)//size
            => Emit(sprite, position, new Vector2(0, 0.5f), 1);

        public async UniTask Emit(Sprite sprite, Vector3 position, Vector3 offset, float spriteSize)//size
        {
            var element = _spritePool.Get();
            element.transform.position = position;
            element.sprite = sprite;
            var maxSpriteDimension = Mathf.Max(sprite.bounds.size.x, sprite.bounds.size.y);
            element.transform.localScale = Vector3.one * spriteSize * _defaultSpriteSize / maxSpriteDimension;
            var duration = 1f;
            element.transform.DOBlendableMoveBy(offset, duration).SetEase(Ease.OutCubic);
            element.DOFade(0, duration).SetDelay(duration / 3).OnComplete(() => _spritePool.Release(element));
        }

        private void OnGet(SpriteRenderer element)
        {
            element.gameObject.SetActive(true);
            element.color = new Color(element.color.r, element.color.g, element.color.b, 1);
        }

        private void OnRelease(SpriteRenderer element)
        {
            element.gameObject.SetActive(false);
        }
    }
}
