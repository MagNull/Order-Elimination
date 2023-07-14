using AI;

namespace GameInventory.Views
{
    public class RoleFilterPlayerInventoryView : PlayerInventoryView
    {
        private Role _currentCharacterRole;

        public void UpdateCharacterRole(Role role) => _currentCharacterRole = role;
        
        public void FilterWithRole()
        {
            foreach (var cell in _cells)
            {
                if (cell.Key.Item == null)
                    continue;
                if (!cell.Key.Item.CanTook[_currentCharacterRole])
                    cell.Value.Disable();
            }
        }
    }
}