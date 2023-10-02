using GameInventory.Items;
using OrderElimination;

namespace GameInventory.Views
{
    public class ItemFilterPlayerInventoryView : PlayerInventoryView
    {
        private IGameCharacterTemplate _currentCharacterData;

        public void UpdateCharacterData(IGameCharacterTemplate data) => _currentCharacterData = data;

        public void FilterWithRole()
        {
            var role = _currentCharacterData.Role;
            foreach (var cell in _cellViews)
            {
                var item = cell.Model.Item;
                if (item is EmptyItem)
                    continue;
                
                if (!item.Data.RoleFilter[role] ||
                    (item.Data.CharacterFilter.Count > 0 && !item.Data.CharacterFilter.Contains(_currentCharacterData)))
                    cell.Disable();
            }
        }
    }
}