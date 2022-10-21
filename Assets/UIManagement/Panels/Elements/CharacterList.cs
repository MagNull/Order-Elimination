using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UIManagement.Debugging;
using UIManagement.Elements;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    [ExecuteInEditMode]
    public class CharacterList : MonoBehaviour
    {
        [SerializeField] private CharacterListElement _elementPrefab;
        private readonly List<CharacterListElement> _characterList = new List<CharacterListElement>();
        public int CharactersCount => _characterList.Count;

        private bool _hasMaintenanceCost = true;
        public bool HasMaintenanceCost
        {
            get => _hasMaintenanceCost;
            set
            {
                _hasMaintenanceCost = value;
                foreach (var e in _characterList)
                {
                    e.HasMaintenanceCost = value;
                }
            }
        }

        private bool _hasParameters = true;
        public bool HasParameters
        {
            get => _hasParameters;
            set
            {
                _hasParameters = value;
                foreach (var e in _characterList)
                {
                    e.HasParameters = value;
                }
            }
        }

        public void Add(params Character[] charactersInfo)
        {
            if (_elementPrefab == null)
                throw new Exception("No given prefab for instancing.");
            foreach (var characterInfo in charactersInfo)
            {
                var newElement = Instantiate(_elementPrefab, transform);
                newElement.UpdateCharacterInfo(characterInfo);
                _characterList.Add(newElement);
                newElement.Destroyed += OnElementDestroyed;
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= CharactersCount || index < 0)
                throw new IndexOutOfRangeException();
            var element = _characterList[index];
            _characterList.RemoveAt(index);
            DestroyImmediate(element.gameObject);
        }

        public void Clear()
        {
            var elementsToRemove = _characterList.ToList();
            _characterList.Clear();
            foreach (var e in elementsToRemove)
            {
                DestroyImmediate(e.gameObject);
            }
        }

        public bool HasForeignChildren => transform.childCount != CharactersCount;

        public void DestroyAllChildrenNotInList()
        {
            if (!HasForeignChildren)
                return;
            var children = new List<Transform>();
            foreach (Transform t in transform)
                children.Add(t);
            foreach (var c in children
                .Select(t => t.gameObject)
                .Except(_characterList.Select(e => e.gameObject)))
            {
                if (c == gameObject)
                    continue;
                DestroyImmediate(c);
            }

        }

        private void OnElementDestroyed(CharacterListElement element)
        {
            if (_characterList.Contains(element))
            {
                _characterList.Remove(element);
            }
            element.Destroyed -= OnElementDestroyed;
        }
    }
}
