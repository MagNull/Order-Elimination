using ItemsLibrary;
using OrderElimination;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace StartSessionMenu
{
    public class StartSessionMenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private int StartMoney = 1000;
        [SerializeField] private Library _library;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var wallet = new Wallet(StartMoney);
            var sceneTransition = new SceneTransition();
            
            builder.RegisterComponent(wallet);
            builder.RegisterComponent(_library);
            builder.RegisterComponent(sceneTransition);
        }
    }
}