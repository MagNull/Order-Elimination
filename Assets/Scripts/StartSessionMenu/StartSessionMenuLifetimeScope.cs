using GameInventory;
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
        [SerializeField]
        private Library _library;

        [SerializeField]
        private CharacterInfoPanel _characterInfoPanel;

        [SerializeField]
        private ScenesMediator _scenesMediatorPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            var sceneTransition = new SceneTransition();
            var mediator = FindObjectOfType<ScenesMediator>() ?? Instantiate(_scenesMediatorPrefab);
            builder.RegisterComponent(_library);
            builder.RegisterComponent(mediator);
            builder.RegisterComponent(sceneTransition);
            builder.RegisterComponent(_characterInfoPanel);
        }
    }
}