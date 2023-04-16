using OrderElimination;
using VContainer;
using VContainer.Unity;

namespace StartSessionMenu
{
    public class StartSessionMenuLifetimeScope : LifetimeScope
    {
        public int StartMoney = 1000;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var wallet = new Wallet(StartMoney);
            builder.RegisterComponent(wallet);
        }
    }
}