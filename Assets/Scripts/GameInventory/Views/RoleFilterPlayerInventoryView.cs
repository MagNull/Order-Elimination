﻿using AI;
using GameInventory.Items;

namespace GameInventory.Views
{
    public class RoleFilterPlayerInventoryView : PlayerInventoryView
    {
        private Role _currentCharacterRole;

        public void UpdateCharacterRole(Role role) => _currentCharacterRole = role;
        
        public void FilterWithRole()
        {
            foreach (var cell in _cellViews)
            {
                if (cell.Model.Item is EmptyItem)
                    continue;
                if (!cell.Model.Item.Data.RoleFilter[_currentCharacterRole])
                    cell.Disable();
            }
        }
    }
}