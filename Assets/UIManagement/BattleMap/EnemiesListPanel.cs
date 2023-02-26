using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UIManagement.Elements
{
    public class EnemiesListPanel : MonoBehaviour
    {
        [SerializeField]
        private CharacterBattleStatsPanel _characterStatsItemPrefab;
        [SerializeField]
        private ScrollRect _charactersScrollRect;
        [SerializeField]
        private RectTransform _listItemsHolder;
        [SerializeField]
        private bool _enemiesInfoByClickingAvailable = false;
        [SerializeField]
        private bool _enemiesInfoByHoldingAvailable = false;
        [SerializeField]
        private float _enemyDisappearTime = 0.3f;
        [SerializeField]
        private Ease _enemyDisappearEase = Ease.Flash;
        private Dictionary<IBattleObject, CharacterBattleStatsPanel> _characterPanels = new();
        //private List<CharacterBattleStatsPanel> _characterPanelsIds = new ();

        [Inject]
        public void Construct(BattleMapView battleMapView)
        {
            battleMapView.CellClicked -= OnCellClicked;
            battleMapView.CellClicked += OnCellClicked;
        }

        public void Populate(BattleCharacterView[] enemies)
        {
            Clear();
            foreach (var enemy in enemies)
            {
                var newStatsItem = Instantiate(_characterStatsItemPrefab, _listItemsHolder);
                newStatsItem.UpdateCharacterInfo(enemy);
                newStatsItem.IsClickingAvatarAvailable = _enemiesInfoByClickingAvailable;
                newStatsItem.IsHoldingAvatarAvailable = _enemiesInfoByHoldingAvailable;
                _characterPanels.Add(enemy.Model, newStatsItem);
                //_characterPanelsIds.Add(newStatsItem);
            }
        }

        public void Clear()
        {
            var elementsToRemove = _characterPanels.Values.ToArray();
            _characterPanels.Clear();
            //_characterPanelsIds.Clear();
            foreach (var e in elementsToRemove)
                Destroy(e.gameObject);
        }

        public void RemoveItem(IBattleObjectView character) => RemoveItem(character.Model);

        public void RemoveItem(IBattleObject characterView)
        {
            if (!_characterPanels.ContainsKey(characterView))
                return;
            var item = _characterPanels[characterView];
            _characterPanels.Remove(characterView);
            //_characterPanelsIds.Remove(item);
            item.transform.localScale = Vector3.one;
            item.transform
                .DOScaleY(0.01f, _enemyDisappearTime)
                .SetEase(_enemyDisappearEase)
                .OnComplete(() => Destroy(item.gameObject));
            //Destroy(item.gameObject);
        }

        public void FocusCharacter(IBattleObjectView character) => FocusCharacter(character.Model);

        public void FocusCharacter(IBattleObject character)
        {
            var target = _characterPanels[character];

            //var elements = new List<CharacterBattleStatsPanel>();
            //Debug.Log(_characterPanelsIds.IndexOf(target));

            //_charactersScrollRect.verticalNormalizedPosition = 1f;// ((RectTransform)target.transform).localPosition.y;
            //0 - bottom 1 - top
            var targetTransform = (RectTransform) target.transform;
            var scrollTransform = (RectTransform) _charactersScrollRect.transform;
            var contentTransform = (RectTransform) _charactersScrollRect.content.transform;
            //targetTransform.offsetMin
            var halfElementHeight = targetTransform.rect.height / 2;
            var elementUpperBorder = targetTransform.position.y + halfElementHeight;
            var elementLowerBorder = targetTransform.position.y - halfElementHeight;

            var halfScrollHeight = scrollTransform.rect.height / 2;
            var scrollUpperBorder = scrollTransform.position.y + halfScrollHeight;
            var scrollLowerBorder = scrollTransform.position.y - halfScrollHeight;

            var halfContentHeight = contentTransform.rect.height / 2;
            var contentUpperBorder = contentTransform.position.y + halfContentHeight;
            var contentLowerBorder = contentTransform.position.y - halfContentHeight;
            //Debug.Log($"Scroll[ {scrollTransform.localPosition.y} ] – Element[ {targetTransform.localPosition.y} ]");
            //Debug.Log($"Scroll[ {scrollLowerBorder} : {scrollUpperBorder} ] – Element[ {elementLowerBorder} : {elementUpperBorder} ]");
            if (elementLowerBorder < scrollLowerBorder)
            {
                var deltaBottom = scrollLowerBorder - elementLowerBorder;
                var scrollValue = deltaBottom / (contentUpperBorder - contentLowerBorder);
                DOTween.To(GetScrollViewPosition, SetScrollViewPosition, scrollValue, 0.2f);
            }
            else if (elementUpperBorder > scrollUpperBorder)
            {
                var deltaTop = scrollUpperBorder - elementUpperBorder;
                var scrollValue = 1 - (deltaTop / (contentUpperBorder - contentLowerBorder));
                DOTween.To(GetScrollViewPosition, SetScrollViewPosition, scrollValue, 0.2f);
            }

            foreach (var e in _characterPanels.Values)
                e.KillHighlightProcess();
            target.Highlight(Color.red);

            float GetScrollViewPosition() => _charactersScrollRect.verticalNormalizedPosition;
            void SetScrollViewPosition(float value) => _charactersScrollRect.verticalNormalizedPosition = value;
        }

        public void LoseFocus()
        {
            _charactersScrollRect.verticalNormalizedPosition = 0;
        }

        private void OnCellClicked(CellView cellView)
        {
            var clickedCell = cellView.Model.Objects;

            foreach (var clickedObject in clickedCell)
            {
                if (clickedObject.Type == BattleObjectType.Ally ||
                    !_characterPanels.ContainsKey(clickedObject)) continue;
                FocusCharacter(clickedObject);
            }
        }
    }
}