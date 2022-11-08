// using OrderElimination;
//
// namespace CharacterAbility
// {
//     public class HealOverTimeEffect : ITickEffect
//     {
//         private readonly IBattleObject _target;
//         private readonly int _heal;
//         private int _duration;
//
//         public HealOverTimeEffect(IBattleObject target, int heal, int duration)
//         {
//             _target = target;
//             _heal = heal;
//             _duration = duration;
//         }
//
//         public void Tick(IReadOnlyBattleStats stats)
//         {
//             _target.TakeHeal(stats.Health * _heal / 100, 100);
//             _duration--;
//             if (_duration <= 0)
//             {
//                 _target.RemoveTickEffect(this);
//             }
//         }   
//     }
// }