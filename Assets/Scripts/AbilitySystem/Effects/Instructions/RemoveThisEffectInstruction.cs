using Cysharp.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class RemoveThisEffectInstruction : IEffectInstruction
    {
        public async UniTask Execute(BattleEffect effect)
        {
            var success = effect.EffectHolder.RemoveEffect(effect);
            if (!success)
                Logging.LogError($"Effect {effect.EffectData.View.Name} couldn't remove itself.");
        }
    }
}
