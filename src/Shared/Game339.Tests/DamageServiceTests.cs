using NUnit.Framework;
using Game339.Shared.Models;
using Game339.Shared.Services.Implementation;
using Game339.Shared.Diagnostics;

namespace Game339.Tests
{
    public class DamageServiceTests
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        public void ApplyDamage_ReducesHealth(int damageModifier)
        {
            var damageService = new DamageService(TestGameLog.Instance, damageModifier);

            var attacker = new Character();
            attacker.Name.Value = "Attacker";
            attacker.Damage.Value = 3;

            int startingHealth = 100;
            var defender = new Character();
            defender.Name.Value = "Defender";
            defender.Health.Value = startingHealth;
            defender.Armor.Value = 0;

            int damage = damageService.CalculateDamage(attacker, defender);
            damageService.ApplyDamage(defender, damage);

            int expectedHealth = startingHealth - damage * damageModifier;
            Assert.That(defender.Health.Value, Is.EqualTo(expectedHealth));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        public void ApplyDamage_WithArmor_ReducesHealthByCalculatedAmount(int damageModifier)
        {
            var damageService = new DamageService(TestGameLog.Instance, damageModifier);

            var attacker = new Character();
            attacker.Name.Value = "Attacker";
            attacker.Damage.Value = 5;

            int startingHealth = 100;
            var defender = new Character();
            defender.Name.Value = "Defender";
            defender.Health.Value = startingHealth;
            defender.Armor.Value = 2;

            int damage = damageService.CalculateDamage(attacker, defender);
            damageService.ApplyDamage(defender, damage);

            Assert.That(damage, Is.EqualTo(3));
            int expectedHealth = startingHealth - damage * damageModifier;
            Assert.That(defender.Health.Value, Is.EqualTo(expectedHealth));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        public void ApplyDamage_DoesNotReduceHealthBelowZero(int damageModifier)
        {
            var damageService = new DamageService(TestGameLog.Instance, damageModifier);

            var attacker = new Character();
            attacker.Name.Value = "Attacker";
            attacker.Damage.Value = 50;

            var defender = new Character();
            defender.Name.Value = "Defender";
            defender.Health.Value = 10;
            defender.Armor.Value = 0;

            int damage = damageService.CalculateDamage(attacker, defender);
            damageService.ApplyDamage(defender, damage);

            Assert.That(defender.Health.Value, Is.EqualTo(0));
        }
    }

    public class TestGameLog : IGameLog
    {
        public static readonly TestGameLog Instance = new TestGameLog();

        public void Info(string message) { }
        public void Warn(string message) { }
        public void Error(string message) { }
    }
}
