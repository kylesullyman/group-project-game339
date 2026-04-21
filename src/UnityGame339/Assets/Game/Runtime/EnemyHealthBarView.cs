using Game339.Shared.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime
{
    public class EnemyHealthBarView : ObserverMonoBehaviour
    {
        [SerializeField] private Slider healthSlider;

        private static ICombatViewModel CombatViewModel => ServiceResolver.Resolve<ICombatViewModel>();

        protected override void Subscribe()
        {
            CombatViewModel.EnemyHealthBar.HealthPercent.ChangeEvent += OnHealthPercentChanged;
        }

        protected override void Unsubscribe()
        {
            CombatViewModel.EnemyHealthBar.HealthPercent.ChangeEvent -= OnHealthPercentChanged;
        }

        private void OnHealthPercentChanged(float percent)
        {
            if (healthSlider == null) return;
            healthSlider.maxValue = 1f;
            healthSlider.value = Mathf.Clamp01(percent);
        }
    }
}
