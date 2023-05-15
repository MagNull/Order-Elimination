using System;
using System.Collections.Generic;
using OrderElimination;
using RoguelikeMap.Points;
using RoguelikeMap.Points.Models;
using UIManagement.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap.Panels
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