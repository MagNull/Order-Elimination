using Cysharp.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public interface IEffectInstruction
    {
        public UniTask Execute(BattleEffect effect);
    }
}
