using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using OrderElimination.MacroGame;
using System.Text.RegularExpressions;
using AI;
using OrderElimination.GameContent;
using System;

namespace OrderElimination
{
    //[Title("Battle Entity Template", "Character", TitleAlignment = TitleAlignments.Centered)]
    [PropertyTooltip("@$value?." + nameof(Name))]
    [CreateAssetMenu(fileName = "new CharacterTemplate", menuName = "OrderElimination/Battle/Character Template")]
    [Serializable]
    public class CharacterTemplate : SerializedScriptableObject, IGameCharacterTemplate, ITemplateAsset
    {

        #region TemplateAsset
        [SerializeField, DisplayAsString(Overflow = true)]
        private Guid _assetId;
        public string AssetName => Name;
        public Sprite AssetIcon => BattleIcon;
        public Guid AssetId => _assetId;
        public void UpdateId(Guid newId) => _assetId = newId;
        #endregion

        [TitleGroup("Visuals", Alignment = TitleAlignments.Centered, BoldTitle = true, Order = 0)]
        [PropertyOrder(0.0f)]
        [SerializeField]
        private string _name;

        [TitleGroup("Visuals")]
        [PreviewField(Alignment = ObjectFieldAlignment.Left, Height = 80)]
        [PropertyOrder(0.1f)]
        [SerializeField]
        private Sprite _viewIcon;

        [TitleGroup("Visuals")]
        [PreviewField(Alignment = ObjectFieldAlignment.Left, Height = 200)]
        [PropertyOrder(0.2f)]
        [SerializeField]
        private Sprite _viewAvatar;

        [TitleGroup("Character Stats", Alignment = TitleAlignments.Centered, BoldTitle = true, Order = 1)]
        [Min(0f)]
        [PropertyOrder(1.0f)]
        [SerializeField]
        private float _maxHealth;
        [TitleGroup("Character Stats")]
        [Min(0f)]
        [PropertyOrder(1.1f)]
        [SerializeField]
        private float _maxArmor;
        [TitleGroup("Character Stats")]
        [Min(0f)]
        [PropertyOrder(1.2f)]
        [SerializeField]
        private float _attackDamage;
        [TitleGroup("Character Stats")]
        [Range(0, 1)]
        [PropertyOrder(1.3f)]
        [SerializeField]
        private float _accuracy;
        [TitleGroup("Character Stats")]
        [Range(0, 1)]
        [PropertyOrder(1.4f)]
        [SerializeField]
        private float _evasion;
        [TitleGroup("Character Stats")]
        [Min(0f)]
        [PropertyOrder(1.5f)]
        [SerializeField]
        private float _maxMovementDistance;

        [TitleGroup("Abilities", Alignment = TitleAlignments.Centered, BoldTitle = true, Order = 2)]
        [PropertyOrder(2.0f)]
        [SerializeReference]
        private ActiveAbilityBuilder[] _activeAbilitiesData;

        [TitleGroup("Abilities")]
        [PropertyOrder(2.1f)]
        [SerializeReference]
        private PassiveAbilityBuilder[] _passiveAbilitiesData;

        [SerializeField]
        private int _price;

        [field: SerializeField] 
        public int Price { get; private set; }
        
        [TitleGroup("Misc", Alignment = TitleAlignments.Centered, BoldTitle = true, Order = 3)]
        [PropertyOrder(3.0f)]
        [field: SerializeField]
        public int Reward { get; private set; }
        [field: SerializeField]
        public Role Role { get; private set; }

        public string Name => _name;
        public Sprite BattleIcon => _viewIcon;
        public Sprite Avatar => _viewAvatar;
        public IReadOnlyGameCharacterStats GetBaseBattleStats() => new GameCharacterStats(
            _maxHealth, _maxArmor, _attackDamage, _accuracy, _evasion, _maxMovementDistance);
        
        public ActiveAbilityBuilder[] GetActiveAbilities() => _activeAbilitiesData.ToArray();
        public PassiveAbilityBuilder[] GetPassiveAbilities() => _passiveAbilitiesData.ToArray();

        //Parses string with hp, dmg, armor, evasion, accuracy respectively.
        [TitleGroup("Character Stats")]
        [PropertyOrder(0)]
        [ShowInInspector]
        [Tooltip("Pattern: \"health damage armor evasion accuracy\"")]
        [Button("Parse string to characteristics", Style = ButtonStyle.Box)]
        private void SetStatsFromTableString(string characteristicsString, bool percentsAsFracture)
        {
            var splitSigns = new char[] { '\t', ' ' };
            var elements = characteristicsString.Split(splitSigns);
            for (var i = 0; i < elements.Length; i++)
            {
                if (float.TryParse(elements[i], out var result))//"0.0" value parsing
                {
                    if (i == 0) _maxHealth = result;
                    else if (i == 1) _attackDamage = result;
                    else if (i == 2) _maxArmor = result;
                    else if (i == 3) _evasion = percentsAsFracture ? result : result / 100;
                    else if (i == 4) _accuracy = percentsAsFracture ? result : result / 100;
                    else return;
                }
                else//"0.0%" value parsing
                {
                    if ((i == 3 || i == 4) 
                        && TryParsePercentPostfixValue(elements[i], out var perValue))
                    {
                        if (i == 3) _evasion = perValue;
                        else if (i == 4) _accuracy = perValue;
                    }
                    else Logging.LogError($"Failed to parse \"{elements[i]}\" value.");
                }
            }

            bool TryParsePercentPostfixValue(string value, out float fractureResult)
            {
                fractureResult = 0;
                var regex = new Regex("(.+)%$");
                var match = regex.Match(value);
                if (match.Success)
                {
                    if (float.TryParse(match.Groups[1].Value, out var parsedPercVal))
                    {
                        fractureResult = parsedPercVal / 100;
                        return true;
                    }
                }
                return false;
            }
        }
    }
}