using ItemsLibrary;
using OrderElimination;
using RoguelikeMap.UI.Characters;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace StartSessionMenu
{
    public class StartSessionMenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private Library _library;
        [SerializeField] private CharacterInfoPanel _characterInfoPanel;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var sceneTransition = new SceneTransition();
            builder.RegisterComponent(_library);
            builder.RegisterComponent(sceneTransition);
            builder.RegisterComponent(_characterInfoPanel);
        }
    }
}