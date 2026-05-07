using Game339.Shared.ViewModels;
using TMPro;
using UnityEngine;

namespace Game.Runtime
{
    public class GoodGuyHpView : ObserverMonoBehaviour
    {
        public TextMeshProUGUI thisIsMyLabel;
        [SerializeField] private Win95HealthBar healthBar;

        private static ICombatViewModel CombatViewModel => ServiceResolver.Resolve<ICombatViewModel>();

        protected override void Subscribe()
        {
            CombatViewModel.PlayerHealthBar.Health.ChangeEvent += OnHealthChanged;
        }

        protected override void Unsubscribe()
        {
            CombatViewModel.PlayerHealthBar.Health.ChangeEvent -= OnHealthChanged;
        }

        private void OnHealthChanged(int health)
        {
            if (thisIsMyLabel != null)
                thisIsMyLabel.text = "" + health;

            if (healthBar != null)
                healthBar.SetHealth(health);
        }
    }
}