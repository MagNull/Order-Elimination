using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using OrderElimination.MacroGame;
using System.Text.RegularExpressions;
using AI;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "new CharacterTemplate", menuName = "Battle/CharacterTemplate")]
    public class CharacterTemplate : SerializedScriptableObject, IGameCharacterTemplate
    {
        [TitleGroup("Visuals", Alignment = TitleAlignments.Centered, BoldTitle = true, Order = 0)]
        [PropertyOrder(0.0f)]
        [SerializeField]
        private string _name;


        [PreviewField(Alignment = ObjectFieldAlignment.Left, Height = 80)]
        [PropertyOrder(0.1f)]
        [SerializeField]
        private Sprite _viewIcon;

        [PreviewField(Alignment = ObjectFieldAlignment.Left, Height = 200)]
        [PropertyOrder(0.2f)]
        [SerializeField]
        private Sprite _viewAvatar;

        [TitleGroup("Character Stats", Alignment = TitleAlignments.Centered, BoldTitle = true, Order = 1)]
        [Min(0f)]
        [PropertyOrder(1.0f)]
        [SerializeField]
        private float _maxHealth;
        [Min(0f)]
        [PropertyOrder(1.1f)]
        [SerializeField]
        private float _maxArmor;
        [Min(0f)]
        [PropertyOrder(1.2f)]
        [SerializeField]
        private float _attackDamage;
        [Range(0, 1)]
        [PropertyOrder(1.3f)]
        [SerializeField]
        private float _accuracy;
        [Range(0, 1)]
        [PropertyOrder(1.4f)]
        [SerializeField]
        private float _evasion;
        [Min(0f)]
        [PropertyOrder(1.5f)]
        [SerializeField]
        private float _maxMovementDistance;

        [TitleGroup("Abilities", Alignment = TitleAlignments.Centered, BoldTitle = true, Order = 2)]
        [PropertyOrder(2.0f)]
        [SerializeReference]
        private ActiveAbilityBuilder[] _activeAbilitiesData;

        [PropertyOrder(2.1f)]
        [SerializeReference]
        private PassiveAbilityBuilder[] _passiveAbilitiesData;
        
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
        [Button("Parse from table string", Style = ButtonStyle.Box)]
        private void SetStatsFromTableString(string characteristicsString, bool percentsAsFracture)
        {
            var elements = characteristicsString.Split('\t');
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

        #region Old
        public void SetLevel(int level)
        {
            //if (_strategyStats.Lvl == level)
            //    return;
            //for (var i = _strategyStats.Lvl; i <= level; i++)
            //    Upgrade();
        }

        public void Heal(int healStat)
        {
            // _battleStats.Health += healStat;
        }

        public void Upgrade()
        {
            //IReadOnlyBattleStats _battleStats = null;
            //var battleStats = new BattleStats(_battleStats)
            //{
            //    Health = _strategyStats.HealthGrowth + _battleStats.UnmodifiedHealth,
            //    UnmodifiedHealth = _strategyStats.HealthGrowth + _battleStats.UnmodifiedHealth,
            //    Armor = _strategyStats.ArmorGrowth + _battleStats.UnmodifiedArmor,
            //    UnmodifiedArmor = _strategyStats.ArmorGrowth + _battleStats.UnmodifiedArmor,
            //    Accuracy = _strategyStats.AccuracyGrowth + _battleStats.UnmodifiedAccuracy,
            //    UnmodifiedAccuracy = _strategyStats.AccuracyGrowth + _battleStats.UnmodifiedAccuracy,
            //    Evasion = _strategyStats.EvasionGrowth + _battleStats.UnmodifiedEvasion,
            //    UnmodifiedEvasion = _strategyStats.EvasionGrowth + _battleStats.UnmodifiedEvasion,
            //    Attack = _strategyStats.AttackGrowth + _battleStats.UnmodifiedAttack,
            //    UnmodifiedAttack = _strategyStats.AttackGrowth + _battleStats.UnmodifiedAttack
            //};
            //_strategyStats.Lvl++;

            //Logging.Log($"Health: Old - {_battleStats.UnmodifiedHealth}, New - {battleStats.UnmodifiedHealth}");
            //Logging.Log($"Health: Old - {_battleStats.UnmodifiedArmor}, New - {battleStats.UnmodifiedArmor}");
            //Logging.Log($"Health: Old - {_battleStats.UnmodifiedAccuracy}, New - {battleStats.UnmodifiedAccuracy}");
            //Logging.Log($"Health: Old - {_battleStats.UnmodifiedEvasion}, New - {battleStats.UnmodifiedEvasion}");
            //Logging.Log($"Health: Old - {_battleStats.UnmodifiedAttack}, New - {battleStats.UnmodifiedAttack}");
            //_battleStats = battleStats;
        }

        private void OnValidate()
        {
            //_battleStats.UnmodifiedHealth = _battleStats.Health;
            //_battleStats.UnmodifiedArmor = _battleStats.Armor;
            //_battleStats.UnmodifiedAttack = _battleStats.Attack;
            //_battleStats.UnmodifiedAccuracy = _battleStats.Accuracy;
            //_battleStats.UnmodifiedEvasion = _battleStats.Evasion;
            //_battleStats.UnmodifiedMovement = _battleStats.Movement;
        }
        #endregion
    }
}