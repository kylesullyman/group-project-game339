using System;
using Game339.Shared.Diagnostics;
using Game339.Shared.Models;

namespace Game339.Shared.Services.Implementation
{
    public class DamageService : IDamageService
    {
        private readonly IGameLog _log;

        public event Action OnDamageApplied;

        public DamageService(IGameLog log)
        {
            _log = log;
        }

        public int CalculateDamage(Character attacker, Character defender)
        {
            int damage = attacker.Damage.Value - defender.Armor.Value;
            if (damage < 0)
                damage = 0;

            _log.Info(attacker.Name.Value + " attacks " + defender.Name.Value + " for " + damage + " damage.");
            return damage;
        }

        public void ApplyDamage(Character defender, int damage)
        {
            int newHealth = defender.Health.Value - damage;
            if (newHealth < 0)
                newHealth = 0;

            OnDamageApplied?.Invoke();
            defender.Health.Value = newHealth;
            _log.Info(defender.Name.Value + " now has " + defender.Health.Value + " health.");
        }
    }
}
