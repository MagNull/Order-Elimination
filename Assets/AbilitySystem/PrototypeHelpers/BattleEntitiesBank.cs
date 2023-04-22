using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AbilitySystem.PrototypeHelpers
{
    public class BattleEntitiesBank
    {
        private readonly Dictionary<IAbilitySystemActor, BattleEntityView> _viewsByEntities = new ();
        private readonly Dictionary<BattleEntityView, IAbilitySystemActor> _entitiesByViews = new ();

        public bool ContainsEntity(IAbilitySystemActor entity) => _viewsByEntities.ContainsKey(entity);
        public IAbilitySystemActor[] GetEntities() => _viewsByEntities.Keys.ToArray();
        public BattleEntityView GetViewByEntity(IAbilitySystemActor entity) => _viewsByEntities[entity];
        public IAbilitySystemActor GetEntityByView(BattleEntityView view) => _entitiesByViews[view];

        public void AddEntity(IAbilitySystemActor entity, BattleEntityView view)
        {
            _viewsByEntities.Add(entity, view);
            _entitiesByViews.Add(view, entity);
        }

        public void RemoveEntity(IAbilitySystemActor entity)
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
