using System;
using System.Collections.Generic;
using DG.Tweening;
using OrderElimination;
using OrderElimination.MacroGame;
using RoguelikeMap.Points;
using RoguelikeMap.UI.Characters;
using Sirenix.OdinInspector;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.SquadInfo
{
    public class Squad : SerializedMonoBehaviour
    {
        private const float IconSize = 50f;
        private const float Duration = 0.5f;
        
        //Заглушка, чтобы не запускаться из другой сцены
        [SerializeField]
        private List<CharacterTemplate> _testSquadMembers;
        [SerializeField] 
        private Transform _iconsMembersOnButton;
        
        private SquadModel _model;
        private SquadCommander _commander;
        private SquadMembersPanel _squadMembersPanel;
        private CharacterCardGenerator _characterCardGenerator;
        private List<CharacterCard> _cardsOnButton = new();
        private ScenesMediator _mediator;

        public int AmountOfCharacters => _model.AmountOfMembers;
        public IReadOnlyList<GameCharacter> Members => _model.Members;
        public IReadOnlyList<GameCharacter> ActiveMembers => _model.ActiveMembers;
        
        public PointModel Point => _model.Point;
        public event Action<Squad> OnSelected;
        public event Action<IReadOnlyList<GameCharacter>> OnUpdateMembers;
        
        [Inject]
        private void Construct(SquadCommander commander, 
            SquadMembersPanel squadMembersPanel, CharacterCardGenerator cardGenerator,
            ScenesMediator scenesMediator)
        {
            _commander = commander;
            _squadMembersPanel = squadMembersPanel;
            _characterCardGenerator = cardGenerator;
            _mediator = scenesMediator;
            
            _commander.SetSquad(this);
            _commander.OnSelected += SetSquadMembers;
            _commander.OnHealAccept += HealCharacters;
        }

        private void Start()
        {
            var characters = _mediator.Get<IEnumerable<GameCharacter>>("player characters");
            _model = new SquadModel(characters, _squadMembersPanel, _mediator);
            Debug.Log("New squad created" % Colorize.Red);
            _model.OnUpdateSquadMembers += GenerateCharactersCard;
            GenerateCharactersCard();
        }

        private void GenerateCharactersCard()
        {
            if (_cardsOnButton.Count > 0)
            {
                foreach(var card in _cardsOnButton)
                    Destroy(card.gameObject);
                _cardsOnButton.Clear();
            }

            foreach (var character in _model.ActiveMembers)
            {
                var card = _characterCardGenerator
                    .GenerateCardIcon(character, _iconsMembersOnButton);
                card.SetImage(character.CharacterData.BattleIcon);
                card.transform.localScale = Vector3.one * 1.3f;
                
                _cardsOnButton.Add(card);
            }
        }

        private void HealCharacters(int amountHeal) => _model.HealCharacters(amountHeal);
        
        public void DistributeExperience(float expirience) => _model.DistributeExperience(expirience);

        private void SetSquadMembers(List<GameCharacter> squadMembers, int countActiveMembers)
        {
            _model.SetSquadMembers(squadMembers, countActiveMembers);
            OnUpdateMembers?.Invoke(ActiveMembers);
        }

        public void Visit(PointModel point)
        {
            UpdatePoint(point);
            MoveAnimation(point.Position);
        }
        
        private void UpdatePoint(PointModel point)
        {
            _commander.SetPoint(point);
            _model.SetPoint(point);
        }
        
        public void MoveAnimation(Vector3 position)
        {
            var target = position +
                         new Vector3(-IconSize,
                             IconSize + 10f);
            transform.DOMove(target, Duration);
        }
        
        private void SetActiveSquadMembers(bool isActive) => _model.SetActivePanel(isActive);
        
        private void OnMouseDown() => Select();
        
        private void Select()
        {
            Logging.Log("Squad selected");
            OnSelected?.Invoke(this);
        }
    }
}