using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace OrderElimination
{
    public class AttackOrder : Order
    {
        private readonly IObjectResolver _objectResolver;

        [Inject]
        public AttackOrder(IPoint target, Squad squad, IObjectResolver objectResolver) : base(target, squad)
        {
            _objectResolver = objectResolver;
        }
        
        public override void Start()
        {
            var battleStatsList = _squad.Members.Cast<IBattleCharacterInfo>().ToList();
            var charactersMediator = _objectResolver.Resolve<CharactersMediator>();
            charactersMediator.SetSquad(battleStatsList);
            charactersMediator.SetEnemies(_target.PointInfo.Enemies);
            charactersMediator.SetPointNumber(_target.PointNumber);
            charactersMediator.PointInfo = _target.PointInfo;
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadBattleMap();
        }

        public override void End()
        {
            _squad.DistributeExperience(base._target.PointInfo.Experience);
        }
    }
}
