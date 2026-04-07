using Game339.Shared.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime
{
    public class EnemyHealthBarView : ObserverMonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private int maxHealth = 10;

        private static GameState GameState => ServiceResolver.Resolve<GameState>();

        protected override void Subscribe()
        {
            GameState.BadGuy.Health.ChangeEvent += OnHealthChanged;
            OnHealthChanged(GameState.BadGuy.Health.Value);
        }

        protected override void Unsubscribe()
        {
            GameState.BadGuy.Health.ChangeEvent -= OnHealthChanged;
        }

        private void OnHealthChanged(int health)
        {
            if (healthSlider == null) return;

            healthSlider.maxValue = 100f;

            float percent = ((float)health / maxHealth) * 100f;
            healthSlider.value = Mathf.Clamp(percent, 0f, 100f);
        }
    }
}