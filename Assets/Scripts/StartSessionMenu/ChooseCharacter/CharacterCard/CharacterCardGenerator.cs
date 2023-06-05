using System;
using OrderElimination;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCardGenerator
    {
        private IObjectResolver _objectResolver;
        private CharacterCardWithHealthBar _characterWithHealthBar;
        private CharacterCardWithCost _characterCardWithCost;
        private CharacterCard _characterIcon;

        [Inject]
        public CharacterCardGenerator(IObjectResolver objectResolver,
            CharacterCardWithHealthBar cardWithHealthBar, CharacterCardWithCost cardWithCost,
            CharacterCard characterIcon)
        {
            _objectResolver = objectResolver;
            _characterWithHealthBar = cardWithHealthBar;
            _characterCardWithCost = cardWithCost;
            _characterIcon = characterIcon;
        }

        public CharacterCardWithHealthBar GenerateCardWithHealthBar(Character character,
            Transform parent, bool isSelected = false) =>
            (CharacterCardWithHealthBar)Generate(character, CharacterCardType.HealthBar, parent, isSelected);
        
        public CharacterCardWithCost GenerateCardWithCost(Character character,
            Transform parent, bool isSelected = false) =>
            (CharacterCardWithCost)Generate(character, CharacterCardType.Cost, parent, isSelected);

        public CharacterCard GenerateCardIcon(Character character,
            Transform parent, bool isSelected = false) => 
            Generate(character, CharacterCardType.Avatar, parent, isSelected);
        
        private CharacterCard Generate(Character character, CharacterCardType type, Transform parent, bool isSelected)
        {
            CharacterCard card = type switch
            {
                CharacterCardType.Cost => _objectResolver.Instantiate(_characterCardWithCost, parent),
                CharacterCardType.HealthBar => _objectResolver.Instantiate(_characterWithHealthBar, parent),
                CharacterCardType.Avatar => _objectResolver.Instantiate(_characterIcon, parent),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            card.InitializeCard(character, isSelected);

            return card;
        }
    }
}