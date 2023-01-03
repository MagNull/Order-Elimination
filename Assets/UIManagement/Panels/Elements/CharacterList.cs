using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
    public class CharacterList : SerializedMonoBehaviour, IEnumerable<CharacterListElement>
    {
        [SerializeField] private CharacterListElement _elementPrefab;
        [ListDrawerSettings(IsReadOnly = true), OdinSerialize]
        private readonly List<CharacterListElement> _characterList = new List<CharacterListElement>();
        public int CharactersCount => _characterList.Count;

        [SerializeField, HideInInspector]
        private bool _hasExperienceRecieved;
        [ShowInInspector]
        public bool HasExperienceRecieved
        {
            get => _hasExperienceRecieved;
            set
            {
                _hasExperienceRecieved = value;
                foreach (var e in _characterList)
                {
                    e.HasExperienceRecieved = value;
                }
            }
        }

        [SerializeField, HideInInspector]
        private bool _hasMaintenanceCost;
        [ShowInInspector]
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

        [SerializeField, HideInInspector]
        private bool _hasParameters;
        [ShowInInspector]
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

        [Button]
        public void Add(params BattleCharacterView[] charactersInfo)
        {
            if (_elementPrefab == null)
                throw new Exception("No given prefab for instancing.");
            foreach (var characterInfo in charactersInfo)
            {
                var newElement = Instantiate(_elementPrefab, transform);
                newElement.UpdateCharacterInfo(characterInfo);
                newElement.HasExperienceRecieved = HasExperienceRecieved;
                newElement.HasMaintenanceCost = HasMaintenanceCost;
                newElement.HasParameters = HasParameters;
                newElement.Destroyed += OnElementDestroyed;
                _characterList.Add(newElement);
            }
        }

        [Button]
        public void RemoveAt(int index)
        {
            if (index >= CharactersCount || index < 0)
                throw new IndexOutOfRangeException();
            var element = _characterList[index];
            _characterList.RemoveAt(index);
            DestroyImmediate(element.gameObject);
        }

        [Button, ShowIf("@Count>0")]
        public void Clear()
        {
            var elementsToRemove = _characterList.ToList();
            _characterList.Clear();
            foreach (var e in elementsToRemove)
            {
                DestroyImmediate(e.gameObject);
            }
        }

        public bool HasForeignChildren => transform.childCount > CharactersCount;

        [Button, ShowIf("HasForeignChildren")]
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

        public IEnumerator<CharacterListElement> GetEnumerator() => _characterList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
