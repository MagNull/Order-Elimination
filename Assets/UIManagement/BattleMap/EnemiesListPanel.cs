using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UIManagement.Elements
{
    public class EnemiesListPanel : MonoBehaviour
    {
        [SerializeField]
        private CharacterBattleStatsPanel _characterStatsItemPrefab;
        [SerializeField]
        private RectTransform _listItemsHolder;
        [SerializeField]
        private bool _enemiesInfoByClickingAvailable = false;
        [SerializeField]
        private float _enemyDisappearTime = 0.3f;
        [SerializeField]
        private Ease _enemyDisappearEase = Ease.Flash;
        private Dictionary<BattleCharacter, CharacterBattleStatsPanel> _characterPanels
            = new Dictionary<BattleCharacter, CharacterBattleStatsPanel>();

        public void Populate(BattleCharacterView[] enemies)
        {
            Clear();
            foreach (var enemy in enemies)
            {
                var newStatsItem = Instantiate(_characterStatsItemPrefab, _listItemsHolder);
                newStatsItem.UpdateCharacterInfo(enemy);
                newStatsItem.IsClickingAvatarAvailable = _enemiesInfoByClickingAvailable;
                _characterPanels.Add(enemy.Model, newStatsItem);
            }
        }

        public void Clear()
        {
            var elementsToRemove = _characterPanels.Values.ToArray();
            _characterPanels.Clear();
            foreach (var e in elementsToRemove)
                Destroy(e.gameObject);
        }

        public void RemoveItem(BattleCharacterView character) => RemoveItem(character.Model);

        public void RemoveItem(BattleCharacter characterView)
        {
            if (!_characterPanels.ContainsKey(characterView))
                return;
            var item = _characterPanels[characterView];
            _characterPanels.Remove(characterView);
            item.transform.localScale = Vector3.one;
            item.transform
                .DOScaleY(0.01f, _enemyDisappearTime)
                .SetEase(_enemyDisappearEase)
                .OnComplete(() => Destroy(item.gameObject));
            //Destroy(item.gameObject);
        }
    } 
}
