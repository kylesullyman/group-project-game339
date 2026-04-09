using Game339.Shared.Models;

namespace Game339.Shared.ViewModels
{
    public class CombatViewModel : ICombatViewModel
    {
        public IHealthBarViewModel PlayerHealthBar { get; }
        public IHealthBarViewModel EnemyHealthBar { get; }
        public ObservableValue<bool> IsPlayerTurn { get; } = new(true);
        public ObservableValue<bool> IsCombatOver { get; } = new(false);
        public ObservableValue<bool> PlayerWon { get; } = new(false);
        public ObservableValue<string> StatusMessage { get; } = new(string.Empty);

        public CombatViewModel(GameState gameState)
        {
            PlayerHealthBar = new HealthBarViewModel(gameState.GoodGuy);
            EnemyHealthBar = new HealthBarViewModel(gameState.BadGuy);
        }

        public void OnCombatStarted(int playerMaxHealth, int enemyMaxHealth)
        {
            PlayerHealthBar.SetMaxHealth(playerMaxHealth);
            EnemyHealthBar.SetMaxHealth(enemyMaxHealth);

            IsCombatOver.Value = false;
            PlayerWon.Value = false;
            IsPlayerTurn.Value = true;
            StatusMessage.Value = "Combat started!";
        }

        public void OnPlayerTurnBegan() => IsPlayerTurn.Value = true;

        public void OnEnemyTurnBegan() => IsPlayerTurn.Value = false;

        public void OnStatusUpdated(string message) => StatusMessage.Value = message;

        public void OnCombatEnded(bool playerWon)
        {
            IsCombatOver.Value = true;
            PlayerWon.Value = playerWon;
            StatusMessage.Value = playerWon ? "You win!" : "You lose!";
        }
    }
}
