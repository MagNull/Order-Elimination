using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class EntityStatusHolder
    {
        private Dictionary<BattleStatus, int> _statusEffects = new();

        public IEnumerable<BattleStatus> ActiveStatuses => _statusEffects.Keys;

        public event Action<BattleStatus> StatusAppeared;
        public event Action<BattleStatus> StatusDisappeared;

        public bool HasStatus(BattleStatus status) => _statusEffects.ContainsKey(status);

        public void IncreaseStatus(BattleStatus status)
        {
            if (!_statusEffects.ContainsKey(status))
            {
                _statusEffects.Add(status, 1);
                StatusAppeared?.Invoke(status);
                return;
            }
            _statusEffects[status]++;
        }

        public bool DecreaseStatus(BattleStatus status)
        {
            if (!_statusEffects.ContainsKey(status)) return false;
            _statusEffects[status]--;
            if (_statusEffects[status] == 0)
            {
                _statusEffects.Remove(status);
                StatusDisappeared?.Invoke(status);
            }
            return true;
        }

        //Shouldn't be allowed? Effects must decrease status when removed.
        private void ClearStatus(BattleStatus status)
        {
            if (!_statusEffects.ContainsKey(status)) return;
            _statusEffects.Remove(status);
            StatusDisappeared?.Invoke(status);
        }
    }

    public enum BattleStatus
    {
        Invisible,
        CantMove,
        //ActiveAbilitiesDisabled
        //PassiveAbilitiesDisabled
        //Invulnerable// - Cant be damaged
    }
}
