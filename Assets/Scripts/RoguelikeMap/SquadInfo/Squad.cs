using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using OrderElimination;
using OrderElimination.MacroGame;
using OrderElimination.SavesManagement;
using RoguelikeMap.Points.Models;
using RoguelikeMap.UI.Characters;
using Sirenix.OdinInspector;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.SquadInfo
{
    public class Squad : SerializedMonoBehaviour
    {
        public float IconSize { get; private set; } = 50f;
        private const float Duration = 0.5f;

        [SerializeField]
        private Transform _iconsMembersOnButton;

        private SquadModel _model;
        private SquadCommander _commander;
        private SquadMembersPanel _squadMembersPanel;
        private CharacterCardGenerator _characterCardGenerator;
        private List<CharacterCard> _cardsOnButton = new();
        private ScenesMediator _mediator;

        public IReadOnlyList<GameCharacter> Members => _model.Members;
        public IReadOnlyList<GameCharacter> ActiveMembers => _model.ActiveMembers;

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

        public void Initialize()
        {
            var progress = _mediator.Get<IPlayerProgress>(MediatorRegistration.Progress);
            List<GameCharacter> characters;//= default test characters
            if (progress != null && progress.CurrentRunProgress != null)
            {
                characters = progress.CurrentRunProgress.PosessedCharacters;
            }
            else
            {
                throw new InvalidOperationException("Invalid progress");
            }
            _model = new SquadModel(characters);
            Debug.Log("New squad created" % Colorize.Red);
            _model.SquadUpdated += OnSquadUpdated;
            OnSquadUpdated(_model);
        }

        private void GenerateCharactersCard()
        {
            if (_cardsOnButton.Count > 0)
            {
                foreach (var card in _cardsOnButton)
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

        private void HealCharacters(int amountHeal)
        {
            foreach (var member in Members)
            {
                member.CurrentHealth += amountHeal;
            }
            foreach (var card in _cardsOnButton)
                card.UpdateColor();
        }

        public void HealCharactersByPercentage(int percentage)
        {
            foreach (var member in Members)
            {
                float healHp = member.CurrentHealth / 100 * percentage;
                member.CurrentHealth += healHp;
            }
            foreach (var card in _cardsOnButton)
                card.UpdateColor();
        }

        public void DamageCharactersByPercentage(int percentage)
        {
            foreach (var member in Members)
            {
                float damageHp = member.CurrentHealth / 100 * percentage;
                member.CurrentHealth -= damageHp;
            }
            foreach (var card in _cardsOnButton)
                card.UpdateColor();
        }

        private void SetSquadMembers(List<GameCharacter> squadMembers)
        {
            _model.SetSquadMembers(squadMembers);
            _mediator.Register(MediatorRegistration.PlayerCharacters, Members.ToArray());
            OnUpdateMembers?.Invoke(ActiveMembers);
        }

        public void AddMembers(IEnumerable<GameCharacter> members)
        {
            _model.Add(members);
            GenerateCharactersCard();
        }

        public async Task Visit(PointModel pointModel)
        {
            await MoveAnimation(pointModel.position);
            _commander.SetPoint(pointModel);
        }

        private async Task MoveAnimation(Vector2 position)
        {
            var target = position +
                         new Vector2(-IconSize,
                             IconSize + 10f);
            await transform.DOMove(target, Duration).AsyncWaitForCompletion();
        }

        public void MoveWithoutAnimation(Vector2 position)
        {
            var target = position +
                         new Vector2(-IconSize,
                             IconSize + 10f);
            transform.position = target;
        }

        public void OpenPanel(bool isActiveAttackButton = true)
        {
            _squadMembersPanel.SetActiveAttackButton(isActiveAttackButton);
            _squadMembersPanel.Open();
        }

        #region UI interaction
        private void OnSquadUpdated(SquadModel model)
        {
            _squadMembersPanel.UpdateMembers(model.ActiveMembers, model.InactiveMembers);
            GenerateCharactersCard();
        }

        //TODO: Set in UnityEvent in SquadMembersButton
        private void SetPanelActive(bool isActive)
        {
            if (isActive)
                _squadMembersPanel.Open();
            else
                _squadMembersPanel.Close();
        }
        #endregion
    }
}