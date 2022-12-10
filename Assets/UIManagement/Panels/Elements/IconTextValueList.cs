using CharacterAbility;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIManagement.Elements;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    [ExecuteInEditMode, RequireComponent(typeof(LayoutGroup))]
    public class IconTextValueList: SerializedMonoBehaviour
    {
        [SerializeField] private IconTextValueElement _elementPrefab;
        [ListDrawerSettings(IsReadOnly = true), OdinSerialize]
        private readonly List<IconTextValueElement> _elements = new List<IconTextValueElement>();
        public IReadOnlyList<IconTextValueElement> Elements => _elements;
        public int Count => _elements.Count;

        private float _iconSize = 64;
        [ShowInInspector]
        public float IconSize
        {
            get => _iconSize;
            set
            {
                _iconSize = value;
                foreach (var e in _elements)
                    e.IconSize = _iconSize;
            }
        }
        [OdinSerialize, HideInInspector]
        private bool _hasIcons;
        [ShowInInspector]
        public bool HasIcons
        {
            get => _hasIcons;
            set
            {
                _hasIcons = value;
                foreach (var e in _elements)
                    e.HasIcon = _hasIcons;
            }
        }

        [OdinSerialize, HideInInspector]
        private bool _hasTexts;
        [ShowInInspector]
        public bool HasTexts
        {
            get => _hasTexts;
            set
            {
                _hasTexts = value;
                foreach (var e in _elements)
                    e.HasText = _hasTexts;
            }
        }

        [OdinSerialize, HideInInspector]
        private bool _hasValues;
        [ShowInInspector]
        public bool HasValues
        {
            get => _hasValues;
            set
            {
                _hasValues = value;
                foreach (var e in _elements)
                    e.HasValue = _hasValues;
            }
        }

        [Button]
        public void Add(Sprite icon = null, string text = "New Text", string value = "0", ValueUnits valueUnits = ValueUnits.None)
        {
            if (_elementPrefab == null)
                throw new Exception("No given prefab for instancing.");
            var newElement = CreateIconTextValueElement(
                transform, 
                icon, 
                text, 
                $"{value}{Localization.Current.GetUnits(valueUnits)}");
            newElement.HasIcon = HasIcons;
            newElement.HasText = HasTexts;
            newElement.HasValue = HasValues;
            newElement.Destroyed += OnElementDestroyed;
            _elements.Add(newElement);
        }
        
        [Button]
        public void RemoveAt(int index)
        {
            if (index >= Count || index < 0)
                throw new IndexOutOfRangeException();
            var element = _elements[index];
            _elements.RemoveAt(index);
            DestroyImmediate(element.gameObject);
        }

        [Button, ShowIf("@Count>0")]
        public void Clear()
        {
            var elementsToRemove = _elements.ToList();
            _elements.Clear();
            foreach (var e in elementsToRemove)
            {
                DestroyImmediate(e.gameObject);
            }
        }

        public bool HasForeignChildren => transform.childCount > Count;

        [Button, ShowIf(nameof(HasForeignChildren))]
        public void DestroyAllChildrenNotInList()
        {
            if (!HasForeignChildren)
                return;
            var children = new List<Transform>();
            foreach (Transform t in transform)
                children.Add(t);
            foreach (var c in children
                .Select(t => t.gameObject)
                .Except(_elements.Select(e => e.gameObject)))
            {
                if (c == gameObject)
                    continue;
                DestroyImmediate(c);
            }

        }

        private void OnElementDestroyed(IconTextValueElement element)
        {
            if (_elements.Contains(element))
            {
                _elements.Remove(element);
            }
            element.Destroyed -= OnElementDestroyed;
        }

        private IconTextValueElement CreateIconTextValueElement(Transform parent, Sprite icon, string text, string value)
        {
            var element = Instantiate(_elementPrefab, parent);
            element.Icon = icon;
            element.Text = text;
            element.Value = value;
            return element;
        }
    }
}
