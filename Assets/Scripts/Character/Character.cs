using System;
using CharacterAbility;
using Inventory_Items;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using Sirenix.Serialization;
using OrderElimination.Domain;
using System.Linq;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "CharacterInfo", menuName = "Battle/Character")]
    public class Character : SerializedScriptableObject, IBattleCharacterInfo, IGameCharacterData
    {
        //New System
        [SerializeField]
        private string _name;
        [PreviewField(Alignment = ObjectFieldAlignment.Left, Height = 80)]
        [SerializeField]
        private Sprite _viewIcon;
        [PreviewField(Alignment = ObjectFieldAlignment.Left, Height = 200)]
        [SerializeField]
        private Sprite _viewAvatar;
        [ShowInInspector, OdinSerialize]
        private BaseBattleStats _baseBattleStats;
        [SerializeReference]
        private ActiveAbilityBuilder[] _activeAbilitiesData;
        [SerializeReference]
        private PassiveAbilityBuilder[] _passiveAbilitiesData;
        private Inventory_Items.Inventory _inventory = new Inventory_Items.Inventory(2);
        [ShowInInspector]
        public Inventory_Items.Inventory Inventory => _inventory;


        public string Name => _name;
        public Sprite BattleIcon => _viewIcon;
        public Sprite Avatar => _viewAvatar;
        public BaseBattleStats BaseBattleStats => _baseBattleStats;
        
        [field: SerializeField]
        public int CostValue { get; private set; }
        public ActiveAbilityBuilder[] GetActiveAbilities() => _activeAbilitiesData.ToArray();
        public PassiveAbilityBuilder[] GetPassiveAbilities() => _passiveAbilitiesData.ToArray();

        #region Old
        public IReadOnlyBattleStats GetBattleStats() => null;
        public AbilityInfo[] GetActiveAbilityInfos() => null;
        public AbilityInfo[] GetPassiveAbilityInfos() => null;
        public StrategyStats GetStrategyStats() => default;

        [Button]
        public void ResetInventory()
        {
            _inventory = new Inventory_Items.Inventory(2);
        }

        public void RaiseExperience(float experience)
        {
            throw new System.NotImplementedException();
        }

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

            //Debug.Log($"Health: Old - {_battleStats.UnmodifiedHealth}, New - {battleStats.UnmodifiedHealth}");
            //Debug.Log($"Health: Old - {_battleStats.UnmodifiedArmor}, New - {battleStats.UnmodifiedArmor}");
            //Debug.Log($"Health: Old - {_battleStats.UnmodifiedAccuracy}, New - {battleStats.UnmodifiedAccuracy}");
            //Debug.Log($"Health: Old - {_battleStats.UnmodifiedEvasion}, New - {battleStats.UnmodifiedEvasion}");
            //Debug.Log($"Health: Old - {_battleStats.UnmodifiedAttack}, New - {battleStats.UnmodifiedAttack}");
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

        public void ResetStats()
        {
            //_strategyStats.Lvl = 1;
        }
        #endregion
    }
}