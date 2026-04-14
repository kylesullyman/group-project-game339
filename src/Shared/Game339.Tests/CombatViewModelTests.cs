using Game339.Shared.Models;
using Game339.Shared.ViewModels;
using Game339.Shared;

namespace Game339.Tests
{
    public class CombatViewModelTests
    {
        private GameState _gameState;
        private CombatViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _gameState = new GameState();
            _gameState.GoodGuy.Name.Value = "Hero";
            _gameState.GoodGuy.Health.Value = 10;
            _gameState.BadGuy.Name.Value = "Enemy";
            _gameState.BadGuy.Health.Value = 8;

            _viewModel = new CombatViewModel(_gameState);
        }

        [Test]
        public void PlayerHealthBar_IsNotNull()
        {
            Assert.That(_viewModel.PlayerHealthBar, Is.Not.Null);
        }

        [Test]
        public void EnemyHealthBar_IsNotNull()
        {
            Assert.That(_viewModel.EnemyHealthBar, Is.Not.Null);
        }

        [Test]
        public void PlayerHealthBar_ReflectsPlayerName()
        {
            Assert.That(_viewModel.PlayerHealthBar.Name.Value, Is.EqualTo("Hero"));
        }

        [Test]
        public void EnemyHealthBar_ReflectsEnemyName()
        {
            Assert.That(_viewModel.EnemyHealthBar.Name.Value, Is.EqualTo("Enemy"));
        }

        [Test]
        public void PlayerHealthBar_ReflectsPlayerHealth()
        {
            Assert.That(_viewModel.PlayerHealthBar.Health.Value, Is.EqualTo(10));
        }

        [Test]
        public void EnemyHealthBar_ReflectsEnemyHealth()
        {
            Assert.That(_viewModel.EnemyHealthBar.Health.Value, Is.EqualTo(8));
        }

        [Test]
        public void OnCombatStarted_SetsHealthBarsToFull()
        {
            _viewModel.OnCombatStarted(playerMaxHealth: 10, enemyMaxHealth: 8);

            Assert.That(_viewModel.PlayerHealthBar.HealthPercent.Value, Is.EqualTo(1.0f));
            Assert.That(_viewModel.EnemyHealthBar.HealthPercent.Value, Is.EqualTo(1.0f));
        }

        [Test]
        public void OnCombatStarted_SetsIsPlayerTurnTrue()
        {
            _viewModel.OnCombatStarted(10, 8);
            Assert.That(_viewModel.IsPlayerTurn.Value, Is.True);
        }

        [Test]
        public void OnCombatStarted_SetsCombatNotOver()
        {
            _viewModel.OnCombatStarted(10, 8);
            Assert.That(_viewModel.IsCombatOver.Value, Is.False);
        }

        [Test]
        public void PlayerHealthBar_HealthPercent_UpdatesWhenHealthChanges()
        {
            _viewModel.OnCombatStarted(playerMaxHealth: 10, enemyMaxHealth: 8);
            _gameState.GoodGuy.Health.Value = 5;
            Assert.That(_viewModel.PlayerHealthBar.HealthPercent.Value, Is.EqualTo(0.5f));
        }

        [Test]
        public void EnemyHealthBar_HealthPercent_UpdatesWhenHealthChanges()
        {
            _viewModel.OnCombatStarted(playerMaxHealth: 10, enemyMaxHealth: 8);
            _gameState.BadGuy.Health.Value = 4;
            Assert.That(_viewModel.EnemyHealthBar.HealthPercent.Value, Is.EqualTo(0.5f));
        }

        [Test]
        public void OnEnemyTurnBegan_SetsIsPlayerTurnFalse()
        {
            _viewModel.OnCombatStarted(10, 8);
            _viewModel.OnEnemyTurnBegan();
            Assert.That(_viewModel.IsPlayerTurn.Value, Is.False);
        }

        [Test]
        public void OnPlayerTurnBegan_SetsIsPlayerTurnTrue()
        {
            _viewModel.OnCombatStarted(10, 8);
            _viewModel.OnEnemyTurnBegan();
            _viewModel.OnPlayerTurnBegan();
            Assert.That(_viewModel.IsPlayerTurn.Value, Is.True);
        }
}
