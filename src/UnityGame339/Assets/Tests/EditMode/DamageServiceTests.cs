using NUnit.Framework;
using Game339.Shared.Models;
using Game339.Shared.Services.Implementation;
using Game339.Shared.Diagnostics;

namespace Game339.Tests
{
    public class DamageServiceTests
    {
        private DamageService _damageService;

        [SetUp]
        public void SetUp()
        {
            _damageService = new DamageService(TestGameLog.Instance);
        }

        [Test]
        public void ApplyDamage_ReducesHealth()
        {
            var attacker = new Character();
            attacker.Name.Value = "Attacker";
            attacker.Damage.Value = 3;

            var defender = new Character();
            defender.Name.Value = "Defender";
            defender.Health.Value = 10;
            defender.Armor.Value = 0;

            int damage = _damageService.CalculateDamage(attacker, defender);
            _damageService.ApplyDamage(defender, damage);

            Assert.That(defender.Health.Value, Is.EqualTo(7));
        }

        [Test]
        public void ApplyDamage_WithArmor_ReducesHealthByCalculatedAmount()
        {
            var attacker = new Character();
            attacker.Name.Value = "Attacker";
            attacker.Damage.Value = 5;

            var defender = new Character();
            defender.Name.Value = "Defender";
            defender.Health.Value = 10;
            defender.Armor.Value = 2;

            int damage = _damageService.CalculateDamage(attacker, defender);
            _damageService.ApplyDamage(defender, damage);

            Assert.That(damage, Is.EqualTo(3));
            Assert.That(defender.Health.Value, Is.EqualTo(7));
        }

        [Test]
        public void ApplyDamage_DoesNotReduceHealthBelowZero()
        {
            var attacker = new Character();
            attacker.Name.Value = "Attacker";
            attacker.Damage.Value = 50;

            var defender = new Character();
            defender.Name.Value = "Defender";
            defender.Health.Value = 10;
            defender.Armor.Value = 0;

            int damage = _damageService.CalculateDamage(attacker, defender);
            _damageService.ApplyDamage(defender, damage);

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