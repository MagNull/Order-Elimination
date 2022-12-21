using CharacterAbility;
using System.Collections;
using System.Collections.Generic;
using UIManagement.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class PassiveSkillDescriptionPanel : UIPanel
    {
        public override PanelType PanelType => PanelType.PassiveSkillsDescription;
        [SerializeField] private PassiveSkillDescriptionCard _descriptionCardPrefab;
        [SerializeField] private RectTransform _descriptionCardsHolder;
        private List<PassiveSkillDescriptionCard> _skillCards = new List<PassiveSkillDescriptionCard>();

        private void Awake()
        {
            Initialize();
        }

        public void SetPassiveSkillsDescription(AbilityView[] passiveSkills)
        {
            var previousCards = _skillCards.ToArray();
            _skillCards.Clear();
            foreach (var card in previousCards)
                Destroy(card.gameObject);
            foreach (var s in passiveSkills)
            {
                var newCard = Instantiate(_descriptionCardPrefab, _descriptionCardsHolder);
                newCard.AssignPassiveSkill(s);
                _skillCards.Add(newCard);
            }
        }
    } 
}
