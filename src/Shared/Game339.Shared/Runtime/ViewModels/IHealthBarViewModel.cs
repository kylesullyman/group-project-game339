namespace Game339.Shared.ViewModels
{
    public interface IHealthBarViewModel
    {
        ObservableValue<string> Name { get; }
        ObservableValue<int> Health { get; }
        ObservableValue<float> HealthPercent { get; }

        void SetMaxHealth(int maxHealth);
    }
}
