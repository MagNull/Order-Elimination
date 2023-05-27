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

        [Inject]
        public CharacterCardGenerator(IObjectResolver objectResolver,
            CharacterCardWithHealthBar cardWithHealthBar, CharacterCardWithCost cardWithCost)
        {
            _objectResolver = objectResolver;
            _characterWithHealthBar = cardWithHealthBar;
            _characterCardWithCost = cardWithCost;
        }

        public CharacterCardWithHealthBar GenerateCardWithHealthBar(Character character,
            Transform parent, bool isSelected = false) =>
            (CharacterCardWithHealthBar)Generate(character, CharacterCardType.HealthBar, parent, isSelected);
        
        public CharacterCardWithCost GenerateCardWithCost(Character character,
            Transform parent, bool isSelected = false) =>
            (CharacterCardWithCost)Generate(character, CharacterCardType.Cost, parent, isSelected);
        
        private CharacterCard Generate(Character character, CharacterCardType type, Transform parent, bool isSelected)
        {
            CharacterCard card = type switch
            {
                CharacterCardType.Cost => _objectResolver.Instantiate(_characterCardWithCost, parent),
                CharacterCardType.HealthBar => _objectResolver.Instantiate(_characterWithHealthBar, parent),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            card.InitializeCard(character, isSelected);

            return card;
        }
    }
}