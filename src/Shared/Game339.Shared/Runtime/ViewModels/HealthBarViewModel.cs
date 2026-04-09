using Game339.Shared.Models;

namespace Game339.Shared.ViewModels
{
    public class HealthBarViewModel : IHealthBarViewModel
    {
        private readonly Character _character;
        private int _maxHealth = 1;

        public ObservableValue<string> Name { get; } = new();
        public ObservableValue<int> Health { get; } = new();
        public ObservableValue<float> HealthPercent { get; } = new();

        public HealthBarViewModel(Character character)
        {
            _character = character;
            _character.Name.ChangeEvent += name => Name.Value = name;
            _character.Health.ChangeEvent += OnHealthChanged;
        }

        public void SetMaxHealth(int maxHealth)
        {
            _maxHealth = maxHealth;
            RefreshPercent();
        }

        private void OnHealthChanged(int health)
        {
            Health.Value = health;
            RefreshPercent();
        }

        private void RefreshPercent()
        {
            HealthPercent.Value = _maxHealth > 0 ? (float)_character.Health.Value / _maxHealth : 0f;
        }
    }
}
