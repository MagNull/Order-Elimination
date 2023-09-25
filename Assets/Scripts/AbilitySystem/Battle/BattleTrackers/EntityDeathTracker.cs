using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class EntityDeathTracker : IBattleTracker
    {
        private const string _isEntityOfType
            = "@" + nameof(TrackingEntityType) + "==" + nameof(EntityType) + ".";

        private HashSet<AbilitySystemActor> _trackingEntities;
        private int _recordedDeaths;
        private IBattleContext _trackingContext;
        private int _awaitedDeathCount = 1;

        [ShowInInspector, OdinSerialize]
        public EntityType TrackingEntityType { get; private set; }

        [ShowIf(_isEntityOfType + nameof(EntityType.Character))]
        [ShowInInspector, OdinSerialize]
        public IGameCharacterTemplate CharacterData { get; private set; }

        [ShowIf(_isEntityOfType + nameof(EntityType.Structure))]
        [ShowInInspector, OdinSerialize]
        public IBattleStructureTemplate StructureData { get; private set; }

        [ShowInInspector, OdinSerialize]
        public int AwaitedDeathCount
        {
            get => _awaitedDeathCount;
            set
            {
                if (value < 1) value = 1;
                _awaitedDeathCount = value;
            }
        }

        public bool IsConditionMet { get; private set; }

        public event Action<IBattleTracker> ConditionMet;
        public event Action<IBattleTracker> ConditionLost;

        public void StartTracking(IBattleContext battleContext)
        {
            StopTracking();
            IsConditionMet = false;
            _trackingEntities = new();
            _trackingContext = battleContext;
            _recordedDeaths = 0;
            _trackingEntities = GetTrackingEntities(_trackingContext.EntitiesBank).ToHashSet();
            foreach (var entity in _trackingEntities)
            {
                entity.Died += OnEntityDeath;
            }
            _trackingContext.EntitiesBank.BankChanged += OnBankChanged;
        }

        public void StopTracking()
        {
            if (_trackingEntities != null)
            {
                foreach (var entity in _trackingEntities)
                {
                    entity.Died -= OnEntityDeath;
                }
                _trackingEntities.Clear();
            }
            if (_trackingContext != null)
            {
                _trackingContext.EntitiesBank.BankChanged -= OnBankChanged;
            }
        }

        private void OnEntityDeath(AbilitySystemActor entity)
        {
            _recordedDeaths++;
            if (_recordedDeaths >= AwaitedDeathCount
                && !IsConditionMet)
            {
                IsConditionMet = true;
                ConditionMet?.Invoke(this);
            }
        }

        private void OnBankChanged(IReadOnlyEntitiesBank bank)
        {
            var newEntities = GetTrackingEntities(bank).Where(e => !_trackingEntities.Contains(e)).ToArray();
            foreach (var entity in newEntities)
            {
                entity.Died += OnEntityDeath;
                _trackingEntities.Add(entity);
            }
        }

        private IEnumerable<AbilitySystemActor> GetTrackingEntities(IReadOnlyEntitiesBank bank)
        {
            var entities = TrackingEntityType switch
            {
                EntityType.Character => bank.GetEntitiesByBasedTemplate(CharacterData),
                EntityType.Structure => bank.GetEntitiesByBasedTemplate(StructureData),
                _ => throw new NotImplementedException(),
            };
            return entities.Where(e => !e.IsDisposedFromBattle);
        }
    }
}
