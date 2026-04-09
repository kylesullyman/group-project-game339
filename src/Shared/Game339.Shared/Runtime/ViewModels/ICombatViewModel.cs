namespace Game339.Shared.ViewModels
{
    public interface ICombatViewModel
    {
        IHealthBarViewModel PlayerHealthBar { get; }
        IHealthBarViewModel EnemyHealthBar { get; }
        ObservableValue<bool> IsPlayerTurn { get; }
        ObservableValue<bool> IsCombatOver { get; }
        ObservableValue<bool> PlayerWon { get; }
        ObservableValue<string> StatusMessage { get; }

        void OnCombatStarted(int playerMaxHealth, int enemyMaxHealth);
        void OnPlayerTurnBegan();
        void OnEnemyTurnBegan();
        void OnStatusUpdated(string message);
        void OnCombatEnded(bool playerWon);
    }
}
