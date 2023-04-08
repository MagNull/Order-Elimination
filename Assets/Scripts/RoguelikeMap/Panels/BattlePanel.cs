using System;
using System.Collections.Generic;
using OrderElimination;
using UIManagement.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap.Panels
{
    public class BattlePanel : IPanel
    {
        [SerializeField] 
        private CharacterAvatarsList _characterList;
        [SerializeField]
        private Button _startAttackButton;
        
        public void UpdateEnemies(List<IBattleCharacterInfo> enemies)
        {
            _characterList.Clear();
            _characterList.Populate(enemies.ToArray());
        }

        public void OnClickAttackButton()
        {
            base.Close();
        }
    }
}