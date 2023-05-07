using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AbilitySystem.PrototypeHelpers
{
    public interface IReadOnlyEntitiesBank
    {
        public bool ContainsEntity(AbilitySystemActor entity);
        public AbilitySystemActor[] GetEntities();
        public BattleEntityView GetViewByEntity(AbilitySystemActor entity);
        public AbilitySystemActor GetEntityByView(BattleEntityView view);
    }

    public class BattleEntitiesBank : IReadOnlyEntitiesBank
    {
        private readonly Dictionary<AbilitySystemActor, BattleEntityView> _viewsByEntities = new ();
        private readonly Dictionary<BattleEntityView, AbilitySystemActor> _entitiesByViews = new ();

        public bool ContainsEntity(AbilitySystemActor entity) => _viewsByEntities.ContainsKey(entity);
        public AbilitySystemActor[] GetEntities() => _viewsByEntities.Keys.ToArray();
        public BattleEntityView GetViewByEntity(AbilitySystemActor entity) => _viewsByEntities[entity];
        public AbilitySystemActor GetEntityByView(BattleEntityView view) => _entitiesByViews[view];

        public void AddEntity(AbilitySystemActor entity, BattleEntityView view)
        {
            _viewsByEntities.Add(entity, view);
            _entitiesByViews.Add(view, entity);
        }

        public void RemoveEntity(AbilitySystemActor entity)
        {
            var view = _viewsByEntities[entity];
            _viewsByEntities.Remove(entity);
            _entitiesByViews.Remove(view);
        }

        public void Clear()
        {
            _viewsByEntities.Clear();
            _entitiesByViews.Clear();
        }
    }
}
