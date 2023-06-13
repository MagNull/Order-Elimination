using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using OrderElimination;
using OrderElimination.MetaGame;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.UI.Characters;
using Sirenix.OdinInspector;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace RoguelikeMap.SquadInfo
{
    public class Squad : SerializedMonoBehaviour
    {
        private const float IconSize = 50f;
        private const float Duration = 0.5f;
        
        //Заглушка, чтобы не запускаться из другой сцены
        [SerializeField]
        private List<Character> _testSquadMembers;
        [SerializeField] 
        private Transform _iconsMembersOnButton;
        
        private SquadModel _model;
        private SquadCommander _commander;
        private SquadMembersPanel _squadMembersPanel;
        private CharacterCardGenerator _characterCardGenerator;
        private List<CharacterCard> _cardsOnButton = new();

        public int AmountOfCharacters => _model.AmountOfMembers;
        public IReadOnlyList<GameCharacter> Members => _model.Members;
        public PointModel Point => _model.Point;
        public event Action<Squad> OnSelected;
        
        [Inject]
        private void Construct(SquadCommander commander, 
            SquadMembersPanel squadMembersPanel, CharacterCardGenerator cardGenerator)
        {
            _commander = commander;
            _squadMembersPanel = squadMembersPanel;
            _characterCardGenerator = cardGenerator;
            
            _commander.SetSquad(this);
            _commander.OnSelected += SetSquadMembers;
            _commander.OnHealAccept += HealCharacters;
        }

        private void Start()
        {
            var characters = GameCharactersFactory.CreateGameEntities(_testSquadMembers);
            if (SquadMediator.CharacterList is not null)
                characters = SquadMediator.CharacterList;
            if(SquadMediator.Stats is null)
                SquadMediator.SetStatsCoefficient(new List<int>(){0, 0, 0, 0, 0});
            _model = new SquadModel(characters, _squadMembersPanel);
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
            => _model.SetSquadMembers(squadMembers, countActiveMembers);

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
            Debug.Log("Squad selected");
            OnSelected?.Invoke(this);
        }
    }
}