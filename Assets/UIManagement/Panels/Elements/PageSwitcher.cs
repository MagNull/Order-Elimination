using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    [ExecuteInEditMode]
    public class PageSwitcher : MonoBehaviour
    {
        [SerializeField] private PageButton _pageButtonPrefab;
        [SerializeField] private GameObject _newPagePrefab;
        [SerializeField] private RectTransform _pageButtonHolder;
        [SerializeField] private RectTransform _pageHolder;
        [SerializeField] private bool _attachOnLoad = true;
        [SerializeField] private Color _unselectedColor = Color.white;
        [SerializeField] private Color _selectedColor = Color.yellow;
        [SerializeField] private Color _disabledColor = Color.gray;
        private readonly Dictionary<PageButton, GameObject> _pages = new Dictionary<PageButton, GameObject>();
        private readonly List<PageButton> _buttonList = new List<PageButton>();
        private readonly Dictionary<PageButton, Image> _cashedImages = new Dictionary<PageButton, Image>();
        private PageButton _selectedPageButton = null;
        private IEnumerable<PageButton> AvailablePages => _buttonList.Where(b => b.Button.interactable);
        public int PageCount => _pages.Count;

        public void ColorizeTest()
        {
            foreach (var e in _pages)
            {
                var color = UnityEngine.Random.ColorHSV(0, 1, 0.8f, 1f, 0.8f, 1f);
                e.Key.TextComponent.color = color;
                if (!e.Value.TryGetComponent(out Image image))
                    image = e.Value.AddComponent<Image>();
                image.color = color;
            }
        }

        public void AddPage(string pageName, GameObject pagePrefab)
        {
            pagePrefab ??= _newPagePrefab;
            var pageButton = Instantiate(_pageButtonPrefab, _pageButtonHolder);
            var page = Instantiate(pagePrefab, _pageHolder);
            pageButton.name = $"Button {pageName}";
            pageButton.Text = page.name = pageName;
            AddExistingPage(pageButton, page);
        }

        public void AddExistingPage(PageButton pageButton, GameObject page)
        {
            _pages.Add(pageButton, page);
            _buttonList.Add(pageButton);
            _cashedImages.Add(pageButton, pageButton.GetComponent<Image>());
            _cashedImages[pageButton].color = _unselectedColor;
            pageButton.Clicked += ShowPage;
            pageButton.Destroyed += OnElementDestroyed;
            page.SetActive(false);
            if (_selectedPageButton == null && pageButton.Button.interactable)
                ShowPage(pageButton);
        }

        public void ShowPage(PageButton pageButton)
        {
            if (pageButton == null)
                return;
            if (!_buttonList.Contains(pageButton))
                throw new ArgumentException();
            if (!pageButton.Button.interactable)
                throw new InvalidOperationException("Page disabled");
            var page = _pages[pageButton];
            foreach (var e in _pages)
            {
                e.Value.gameObject.SetActive(false);
            }
            page.gameObject.SetActive(true);
            _selectedPageButton = pageButton;
            UpdatePageButtonColors();
        }

        public void ShowPage(int index) => ShowPage(_buttonList[index]);

        public bool ShowNextAvailablePage(bool isCycled)
        {
            var availablePages = AvailablePages.ToList();
            var currentPageIndex = availablePages.IndexOf(_selectedPageButton);
            if (currentPageIndex == -1)
                throw new InvalidOperationException();
            var nextPageIndex = currentPageIndex + 1;
            if (nextPageIndex >= availablePages.Count)
            {
                if (isCycled)
                {
                    ShowPage(availablePages[nextPageIndex % availablePages.Count]);
                    return true;
                }
                return false;
            }
            ShowPage(availablePages[nextPageIndex]);
            return true;
        }

        public bool ShowPreviousAvailablePage(bool isCycled)
        {
            var availablePages = AvailablePages.ToList();
            var currentPageIndex = availablePages.IndexOf(_selectedPageButton);
            if (currentPageIndex == -1)
                throw new InvalidOperationException();
            var previousPageIndex = currentPageIndex - 1;
            if (previousPageIndex < 0)
            {
                if (isCycled)
                {
                    ShowPage(availablePages[(previousPageIndex + availablePages.Count) % availablePages.Count]);
                    return true;
                }
                return false;
            }
            ShowPage(availablePages[previousPageIndex]);
            return true;
        }

        private void UpdatePageButtonColors()
        {
            foreach (var pageButton in _buttonList)
            {
                _cashedImages[pageButton].color = pageButton == _selectedPageButton ? _selectedColor : _unselectedColor;
                if (!pageButton.Button.interactable)
                    _cashedImages[pageButton].color = _disabledColor;
            }
        }

        public void DisablePage(PageButton pageButton)
        {
            pageButton.Button.interactable = false;
            _cashedImages[pageButton].color = _disabledColor;
            if (pageButton == _selectedPageButton)
                ShowPage(AvailablePages.First());
        }

        public void DisablePage(int index) => DisablePage(_buttonList[index]);

        public void EnablePage(PageButton pageButton)
        {
            pageButton.Button.interactable = true;
            _cashedImages[pageButton].color = pageButton == _selectedPageButton ? _selectedColor : _unselectedColor;
        }

        public void EnablePage(int index) => EnablePage(_buttonList[index]);

        public void RemoveAt(int index)
        {
            if (index >= PageCount || index < 0)
                throw new IndexOutOfRangeException();
            var pageButton = _buttonList[index];
            var page = _pages[pageButton];
            _buttonList.RemoveAt(index);
            _pages.Remove(pageButton);
            _cashedImages.Remove(pageButton);
            DestroyImmediate(pageButton.gameObject);
            DestroyImmediate(page.gameObject);
        }

        public void Clear()
        {
            var buttonsToRemove = new List<GameObject>();
            var pagesToRemove = new List<GameObject>();
            foreach (var button in _buttonList)
            {
                var page = _pages[button];
                pagesToRemove.Add(page.gameObject);
                buttonsToRemove.Add(button.gameObject);
            }
            foreach (var e in buttonsToRemove.Union(pagesToRemove))
            {
                DestroyImmediate(e);
                DestroyImmediate(e);
            }
            _pages.Clear();
            _buttonList.Clear();
            _cashedImages.Clear();
        }

        public bool HasForeignChildren => 
            _pageButtonHolder.childCount != PageCount 
            || _pageHolder.childCount != PageCount;

        public void DestroyAllChildrenNotInList()
        {
            if (!HasForeignChildren)
                return;
            var buttonsToRemove = new List<PageButton>();
            var pagesToRemove = new List<GameObject>();

            var buttonHolderChildren = new List<Transform>();
            var pageHolderChildren = new List<Transform>();
            foreach (Transform t in _pageButtonHolder)
                buttonHolderChildren.Add(t);
            foreach (Transform t in _pageHolder)
                pageHolderChildren.Add(t);
            foreach (var c in buttonHolderChildren
                .Select(t => t.gameObject)
                .Except(_buttonList.Select(e => e.gameObject)))
            {
                if (c == gameObject)
                    continue;
                DestroyImmediate(c);
            }
            foreach (var c in pageHolderChildren
                .Select(t => t.gameObject)
                .Except(_pages.Values.Select(e => e.gameObject)))
            {
                if (c == gameObject)
                    continue;
                DestroyImmediate(c);
            }
        }

        public void AttachForeignChildren()
        {
            var buttonHolderChildren = _pageButtonHolder.GetComponentsInChildren<PageButton>().Except(_buttonList).ToList();
            var pageHolderChildren = new List<GameObject>();
            foreach (Transform t in _pageHolder)
                pageHolderChildren.Add(t.gameObject);
            pageHolderChildren.Remove(gameObject);
            for (var i = 0; i < Mathf.Min(buttonHolderChildren.Count, pageHolderChildren.Count); i++)
            {
                AddExistingPage(buttonHolderChildren[i], pageHolderChildren[i]);
            }
            ShowPage(_selectedPageButton);
        }

        private void OnElementDestroyed(PageButton element)
        {
            if (_buttonList.Contains(element))
            {
                _buttonList.Remove(element);
            }
            _pages.Remove(element, out var page);
            DestroyImmediate(page);
            _cashedImages.Remove(element);
            if (_selectedPageButton == element)
            {
                var newPageButton = _buttonList.FirstOrDefault(b => b.Button.interactable);
                _selectedPageButton = newPageButton;
                ShowPage(newPageButton);
            }
            element.Destroyed -= OnElementDestroyed;
        }

        private void Awake()
        {
            if (_attachOnLoad)
                AttachForeignChildren();
        }
    }
}
