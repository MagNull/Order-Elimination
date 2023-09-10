using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public enum BattleStatus
    {
        Invisible = 1,
        CantMove = 2,
        Invulnerable = 4, // - Cant be damaged
        Flying = 8, // - can move freely
        ActiveAbilitiesDisabled = 16,
        PassiveAbilitiesDisabled = 32,
        //CantDie = 64, // - TakesDamage but health cant get lower than 1
    }

    public class EntityStatusHolder
    {
        private Dictionary<BattleStatus, int> _statusEffects = new();

        public IEnumerable<BattleStatus> ActiveStatuses => _statusEffects.Keys.Where(s => _statusEffects[s] > 0);

        public event Action<BattleStatus> StatusAppeared;
        public event Action<BattleStatus> StatusDisappeared;

        public bool HasStatus(BattleStatus status) => _statusEffects.ContainsKey(status) && _statusEffects[status] > 0;

        public void IncreaseStatus(BattleStatus status)
        {
            if (!_statusEffects.ContainsKey(status))
            {
                _statusEffects.Add(status, 0);
            }
            _statusEffects[status]++;
            Logging.Log($"{status}+", Colorize.Cyan);
            if (_statusEffects[status] > 0)
                StatusAppeared?.Invoke(status);

        }

        public bool DecreaseStatus(BattleStatus status)
        {
            if (!_statusEffects.ContainsKey(status))
            {
                Logging.LogError("Attempt to decrease non-existing status");
                return false;
            }
            _statusEffects[status]--;
            Logging.Log($"{status}-", Colorize.Cyan);
            if (_statusEffects[status] == 0)
            {
                //_statusEffects.Remove(status);//Can we go minus? (if Remove() -> no)
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

        public override string ToString()
        {
            return $"Statuses[{_statusEffects.Count}]: {string.Join(", ", _statusEffects.Select(e => $"[{e.Key}:{e.Value}]"))}";
        }
    }
}
