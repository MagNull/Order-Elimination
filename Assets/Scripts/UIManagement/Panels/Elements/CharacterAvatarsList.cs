using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using TMPro;
using UIManagement.Debugging;
using UIManagement.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    [ExecuteInEditMode]
    internal partial class CharacterAvatarsList : MonoBehaviour, IEnumerable<CharacterClickableAvatar>
    {
        [SerializeField] private CharacterClickableAvatar _elementPrefab;
        [SerializeField] private RectTransform _elementsHolder;
        [SerializeReference]
        private List<CharacterClickableAvatar> _characterList = new List<CharacterClickableAvatar>();
        public IReadOnlyList<CharacterClickableAvatar> Elements => _characterList;
        public int ElementsCount => _characterList.Count;

        public event Action<CharacterClickableAvatar> ElementClicked;
        public event Action<CharacterClickableAvatar> ElementHolded;

        [Button]
        public void Populate(Character[] charactersInfo)
        {
            if (_elementPrefab == null)
                throw new Exception("No given prefab for instancing.");
            foreach (var characterInfo in charactersInfo)
            {
                var newElement = Instantiate(_elementPrefab, _elementsHolder);
                newElement.UpdateCharacterInfo(characterInfo);
                newElement.Clicked -= OnElementClicked;
                newElement.Holded -= OnElementHolded;
                newElement.Destroyed -= OnElementDestroyed;
                newElement.Clicked += OnElementClicked;
                newElement.Holded += OnElementHolded;
                newElement.Destroyed += OnElementDestroyed;
                _characterList.Add(newElement);
            }
        }
        
        [Button]
        public void Populate(IBattleCharacterInfo[] charactersInfo)
        {
            if (_elementPrefab == null)
                throw new Exception("No given prefab for instancing.");
            foreach (var characterInfo in charactersInfo)
            {
                var newElement = Instantiate(_elementPrefab, _elementsHolder);
                newElement.UpdateCharacterInfo(characterInfo);
                newElement.Clicked -= OnElementClicked;
                newElement.Holded -= OnElementHolded;
                newElement.Destroyed -= OnElementDestroyed;
                newElement.Clicked += OnElementClicked;
                newElement.Holded += OnElementHolded;
                newElement.Destroyed += OnElementDestroyed;
                _characterList.Add(newElement);
            }
        }

        private void OnElementClicked(CharacterClickableAvatar element) => ElementClicked?.Invoke(element);

        private void OnElementHolded(CharacterClickableAvatar element) => ElementHolded?.Invoke(element);

        [Button]
        public void RemoveAt(int index)
        {
            if (index >= ElementsCount || index < 0)
                throw new IndexOutOfRangeException();
            var element = _characterList[index];
            _characterList.RemoveAt(index);
            DestroyImmediate(element.gameObject);
        }

        [Button, ShowIf("@" + nameof(ElementsCount) + ">0")]
        public void Clear()
        {
            var elementsToRemove = _characterList.ToList();
            _characterList.Clear();
            foreach (var e in elementsToRemove)
            {
                DestroyImmediate(e.gameObject);
            }
        }


        private void OnElementDestroyed(CharacterClickableAvatar element)
        {
            if (_characterList.Contains(element))
            {
                _characterList.Remove(element);
            }
            element.Clicked -= OnElementClicked;
            element.Holded -= OnElementHolded;
            element.Destroyed -= OnElementDestroyed;
        }
    }

    partial class CharacterAvatarsList : IEnumerable<CharacterClickableAvatar>
    {

        public IEnumerator<CharacterClickableAvatar> GetEnumerator() => _characterList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
