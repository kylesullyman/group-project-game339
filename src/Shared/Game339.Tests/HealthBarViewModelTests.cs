using Game339.Shared.Models;
using Game339.Shared.ViewModels;

namespace Game339.Tests
{
    public class HealthBarViewModelTests
    {
        private Character _character;
        private HealthBarViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _character = new Character();
            _character.Name.Value = "Hero";
            _character.Health.Value = 10;

            _viewModel = new HealthBarViewModel(_character);
        }

        [Test]
        public void Name_ReflectsCharacterName()
        {
            Assert.That(_viewModel.Name.Value, Is.EqualTo("Hero"));
        }

        [Test]
        public void Health_ReflectsCharacterHealth()
        {
            Assert.That(_viewModel.Health.Value, Is.EqualTo(10));
        }

        [Test]
        public void HealthPercent_IsZeroBeforeMaxHealthSet()
        {
            Assert.That(_viewModel.HealthPercent.Value, Is.EqualTo(0f));
        }

        [Test]
        public void SetMaxHealth_SetsHealthPercentToFull()
        {
            _viewModel.SetMaxHealth(10);
            Assert.That(_viewModel.HealthPercent.Value, Is.EqualTo(1.0f));
        }

        [Test]
        public void HealthPercent_UpdatesWhenCharacterHealthChanges()
        {
            _viewModel.SetMaxHealth(10);
            _character.Health.Value = 5;
            Assert.That(_viewModel.HealthPercent.Value, Is.EqualTo(0.5f));
        }

        [Test]
        public void Health_UpdatesWhenCharacterHealthChanges()
        {
            _character.Health.Value = 7;
            Assert.That(_viewModel.Health.Value, Is.EqualTo(7));
        }

        [Test]
        public void Name_UpdatesWhenCharacterNameChanges()
        {
            _character.Name.Value = "New Name";
            Assert.That(_viewModel.Name.Value, Is.EqualTo("New Name"));
        }

        [Test]
        public void HealthPercent_IsZeroWhenHealthIsZero()
        {
            _viewModel.SetMaxHealth(10);
            _character.Health.Value = 0;
            Assert.That(_viewModel.HealthPercent.Value, Is.EqualTo(0f));
        }

        [TestCase(10, 10, 1.0f)]
        [TestCase(5, 10, 0.5f)]
        [TestCase(0, 10, 0.0f)]
        [TestCase(3, 4, 0.75f)]
        public void HealthPercent_CorrectForVariousValues(int health, int maxHealth, float expected)
        {
            _character.Health.Value = health;
            _viewModel.SetMaxHealth(maxHealth);
            Assert.That(_viewModel.HealthPercent.Value, Is.EqualTo(expected).Within(0.001f));
        }
    }
}
