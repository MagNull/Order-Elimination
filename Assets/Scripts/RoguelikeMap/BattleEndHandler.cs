using GameInventory;
using OrderElimination;
using OrderElimination.Battle;
using StartSessionMenu;
using VContainer;

namespace RoguelikeMap
{
    public class BattleEndHandler
    {
        private ScenesMediator _scenesMediator;
        private Inventory _inventory;
        private Wallet _wallet;

        [Inject]
        public void Construct(ScenesMediator scenesMediator, Wallet wallet, Inventory inventory)
        {
            _scenesMediator = scenesMediator;
            _wallet = wallet;
            _inventory = inventory;
        }

        public void Start()
        {
            if (!_scenesMediator.Contains<BattleResults>("battle results"))
                return;
            var results = _scenesMediator.Get<BattleResults>("battle results");
            _scenesMediator.Unregister("battle results");
            
            _wallet.AddMoney(results.MoneyReward);
            foreach (var item in results.ItemsReward)
            {
                _inventory.AddItem(item);
            }
        }
    }
}