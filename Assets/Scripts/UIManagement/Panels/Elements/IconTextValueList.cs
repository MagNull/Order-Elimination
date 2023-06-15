using CharacterAbility;
using OrderElimination.Localization;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    [ExecuteInEditMode]
    public class IconTextValueList: MonoBehaviour
    {
        [SerializeField] private IconTextValueElement _elementPrefab;
        [SerializeReference, OnValueChanged(nameof(OnListUpdated))]
        private List<IconTextValueElement> _attachedElements = new List<IconTextValueElement>();
        public IReadOnlyList<IconTextValueElement> Elements => _attachedElements;
        public int Count => _attachedElements.Count;

        [SerializeField, HideInInspector]
        private float _iconSize = 64;
        [SerializeField, HideInInspector]
        private bool _hasIcons;
        [SerializeField, HideInInspector]
        private bool _hasTexts;
        [SerializeField, HideInInspector]
        private bool _hasValues;

        [ShowInInspector]
        public float IconSize
        {
            get => _iconSize;
            set
            {
                _iconSize = value;
                foreach (var e in _attachedElements)
                    e.IconSize = _iconSize;
            }
        }
        [ShowInInspector]
        public bool HasIcons
        {
            get => _hasIcons;
            set
            {
                _hasIcons = value;
                foreach (var e in _attachedElements)
                    e.HasIcon = _hasIcons;
            }
        }
        [ShowInInspector]
        public bool HasTexts
        {
            get => _hasTexts;
            set
            {
                _hasTexts = value;
                foreach (var e in _attachedElements)
                    e.HasText = _hasTexts;
            }
        }
        [ShowInInspector]
        public bool HasValues
        {
            get => _hasValues;
            set
            {
                _hasValues = value;
                foreach (var e in _attachedElements)
                    e.HasValue = _hasValues;
            }
        }

        [Button]
        public void Add(Sprite icon = null, string text = "New Text", string value = "0", ValueUnits valueUnits = ValueUnits.None)
        {
            if (_elementPrefab == null)
                Logging.LogException( new Exception("No prefab given for instancing."));
            _attachedElements.Add(
                CreateIconTextValueElement(transform, icon, text, $"{value}{Localization.Current.GetUnits(valueUnits)}"));
            OnListUpdated();
        }

        [Button, ShowIf("@Count>0")]
        public void Clear()
        {
            var elementsToRemove = _attachedElements.ToArray();
            _attachedElements.Clear();
            foreach (var e in elementsToRemove)
            {
                DestroyImmediate(e.gameObject);
            }
        }

        public bool HasForeignChildren => transform.childCount > Count;

        [Button, ShowIf(nameof(HasForeignChildren))]
        public void DestroyNotAttachedChildren()
        {
            if (!HasForeignChildren)
                return;
            var children = new List<Transform>();
            foreach (Transform t in transform)
                children.Add(t);
            foreach (var c in children
                .Select(t => t.gameObject)
                .Except(_attachedElements.Select(e => e.gameObject)))
            {
                if (c == gameObject)
                    continue;
                DestroyImmediate(c);
            }

        }

        private void OnListUpdated()
        {
            foreach (var e in _attachedElements)
            {
                e.HasIcon = HasIcons; e.HasText = HasTexts; e.HasValue = HasValues;
                e.Destroyed -= OnElementDestroyed;
                e.Destroyed += OnElementDestroyed;
            }
        }

        private void OnElementDestroyed(IconTextValueElement element)
        {
            if (_attachedElements.Contains(element))
            {
                _attachedElements.Remove(element);
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
