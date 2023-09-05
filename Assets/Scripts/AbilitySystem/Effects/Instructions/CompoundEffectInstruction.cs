using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public class CompoundEffectInstruction : IEffectInstruction
    {
        [ShowInInspector, OdinSerialize]
        private List<IEffectInstruction> _instructions = new();

        public async UniTask Execute(BattleEffect effect)
        {
            foreach (var instruction in _instructions)
            {
                await instruction.Execute(effect);
            }
        }
    }
}
