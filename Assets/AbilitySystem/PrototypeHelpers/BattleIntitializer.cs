using OrderElimination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VContainer;

namespace Assets.AbilitySystem.PrototypeHelpers
{
    public class BattleIntitializer
    {
        private BattleMapDirector _battleMapDirector;
        private CharacterArrangeDirector _characterArrangeDirector;
        private CharactersMediator _characterMediator;
        private 

        private void Construct(IObjectResolver objectResolver)
        {
            _battleMapDirector = objectResolver.Resolve<BattleMapDirector>();
            _characterArrangeDirector = objectResolver.Resolve<CharacterArrangeDirector>();
            _characterMediator = objectResolver.Resolve<CharactersMediator>();
        }
    }
}
