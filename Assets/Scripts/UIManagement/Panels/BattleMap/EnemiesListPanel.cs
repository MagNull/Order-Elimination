using DG.Tweening;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
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
        private Dictionary<AbilitySystemActor, CharacterBattleStatsPanel> _entitiesPanels = new();
        private IBattleContext _battleContext;
        //private List<CharacterBattleStatsPanel> _characterPanelsIds = new ();

        [Inject]
        public void Construct(IObjectResolver objectResolver)
        {
            _battleContext = objectResolver.Resolve<IBattleContext>();
            var battleMapView = objectResolver.Resolve<BattleMapView>();
            battleMapView.CellClicked -= OnCellClicked;
            battleMapView.CellClicked += OnCellClicked;
            _battleContext.EntitiesBank.BankChanged += OnEntitiesBankChanged;
        }

        private void OnEntitiesBankChanged(IReadOnlyEntitiesBank bank)
        {
            var enemies = bank.GetEntities().Where(e => IsEnemyCharacter(e));
            if (_entitiesPanels.Count == 0)
            {
                Populate(enemies);
                return;
            }
            var newEnemies = enemies.Except(_entitiesPanels.Keys).ToArray();
            var disappearedEnemies = _entitiesPanels.Keys.Except(enemies).ToArray();
            foreach (var newEnemy in newEnemies)
            {
                Add(newEnemy);
            }
            foreach (var disappearedEntity in disappearedEnemies)
            {
                RemoveItem(disappearedEntity);
            }
        }

        public void Populate(IEnumerable<AbilitySystemActor> enemies)
        {
            Clear();
            foreach (var enemy in enemies)
            {
                Add(enemy);
            }
        }

        private void Add(AbilitySystemActor enemy)
        {
            var newStatsItem = Instantiate(_characterStatsItemPrefab, _listItemsHolder);
            newStatsItem.IsClickingAvatarAvailable = _enemiesInfoByClickingAvailable;
            newStatsItem.IsHoldingAvatarAvailable = _enemiesInfoByHoldingAvailable;
            var view = _battleContext.EntitiesBank.GetViewByEntity(enemy);
            newStatsItem.UpdateEntityInfo(view);
            _entitiesPanels.Add(enemy, newStatsItem);
        }

        public void Clear()
        {
            var elementsToRemove = _entitiesPanels.Values.ToArray();
            _entitiesPanels.Clear();
            //_characterPanelsIds.Clear();
            foreach (var e in elementsToRemove)
                Destroy(e.gameObject);
        }

        public void RemoveItem(AbilitySystemActor entity)
        {
            if (!_entitiesPanels.ContainsKey(entity))
                return;
            var item = _entitiesPanels[entity];
            _entitiesPanels.Remove(entity);
            //_characterPanelsIds.Remove(item);
            item.transform.localScale = Vector3.one;
            item.transform
                .DOScaleY(0.01f, _enemyDisappearTime)
                .SetEase(_enemyDisappearEase)
                .OnComplete(() => Destroy(item.gameObject));
            //Destroy(item.gameObject);
        }

        public void FocusEntity(AbilitySystemActor entity)
        {
            var target = _entitiesPanels[entity];

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

            foreach (var e in _entitiesPanels.Values)
                e.DOComplete();
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
            var enemies = cellView.Model.GetContainingEntities()
                .Where(e => IsEnemyCharacter(e))
                .Where(e => !e.StatusHolder.HasStatus(BattleStatus.Invisible))
                .Where(e => _entitiesPanels.ContainsKey(e))
                .ToArray();
            if (enemies.Length == 0)
                return;
            var firstVisibleEnemy = enemies.First();
            FocusEntity(firstVisibleEnemy);
        }

        private bool IsEnemyCharacter(AbilitySystemActor entity) => 
            entity.EntityType == EntityType.Character
            && _battleContext.GetRelationship(BattleSide.Player, entity.BattleSide) == BattleRelationship.Enemy;
    }
}