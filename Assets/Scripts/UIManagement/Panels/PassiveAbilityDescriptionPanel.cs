using OrderElimination.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UIManagement.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class PassiveAbilityDescriptionPanel : UIPanel
    {
        public override PanelType PanelType => PanelType.PassiveAbilityDescription;
        [SerializeField] private PassiveAbilityDescriptionCard _descriptionCardPrefab;
        [SerializeField] private RectTransform _descriptionCardsHolder;
        private List<PassiveAbilityDescriptionCard> _skillCards = new List<PassiveAbilityDescriptionCard>();

        private void Awake()
        {
            Initialize();
        }

        public void UpdateAbilitiesDescription(IEnumerable<IPassiveAbilityData> abilities)
        {
            var previousCards = _skillCards.ToArray();
            _skillCards.Clear();
            foreach (var card in previousCards)
                Destroy(card.gameObject);
            foreach (var a in abilities)
            {
                var newCard = Instantiate(_descriptionCardPrefab, _descriptionCardsHolder);
                newCard.UpdateAbilityData(a.View.Name, a.View.Icon, a.View.Description);
                _skillCards.Add(newCard);
            }
        }
    } 
}
