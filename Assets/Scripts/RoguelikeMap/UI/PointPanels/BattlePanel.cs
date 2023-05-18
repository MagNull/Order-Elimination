using System;
using System.Collections.Generic;
using UIManagement.Elements;
using UnityEngine;

namespace RoguelikeMap.UI.PointPanels
{
    public class BattlePanel : Panel
    {
        [SerializeField] 
        private CharacterAvatarsList _characterList;
        
        public event Action OnStartAttack;
        
        public void UpdateEnemies(IReadOnlyList<IBattleCharacterInfo> enemies)
        {
            _characterList.Clear();
            _characterList.Populate(enemies);
        }

        public void OnClickAttackButton()
        {
            OnStartAttack?.Invoke();
            base.Close();
        }
    }
}