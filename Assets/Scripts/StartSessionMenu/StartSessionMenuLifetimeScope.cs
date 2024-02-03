using GameInventory;
using ItemsLibrary;
using OrderElimination;
using OrderElimination.SavesManagement;
using RoguelikeMap.UI.Characters;
using Sirenix.OdinInspector;
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

        [SerializeField]
        private PlayerProgressManager _playerProgressManager;

        protected override void Configure(IContainerBuilder builder)
        {
            var sceneTransition = new SceneTransition();
            var mediator = FindObjectOfType<ScenesMediator>() ?? Instantiate(_scenesMediatorPrefab);
            builder.RegisterComponent(_library);
            builder.RegisterComponent(mediator);
            builder.RegisterComponent(sceneTransition);
            builder.RegisterComponent(_characterInfoPanel);
            builder.RegisterComponent(_playerProgressManager).AsImplementedInterfaces();
            mediator.Register<IPlayerProgressManager>("progress manager", _playerProgressManager);
        }
    }
}