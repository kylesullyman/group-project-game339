using Game339.Shared.Models;
using TMPro;

namespace Game.Runtime
{
    public class BadGuyHpView : ObserverMonoBehaviour
    {
        private static GameState GameState => ServiceResolver.Resolve<GameState>();

        public TextMeshProUGUI thisIsMyLabel;
        public Win95HealthBar healthBar;

        protected override void Subscribe()
        {
            GameState.BadGuy.Health.ChangeEvent += OnBadGuyHealthChange;
            OnBadGuyHealthChange(GameState.BadGuy.Health.Value);
        }

        protected override void Unsubscribe()
        {
            GameState.BadGuy.Health.ChangeEvent -= OnBadGuyHealthChange;
        }

        private void OnBadGuyHealthChange(int health)
        {
            if (thisIsMyLabel != null)
                thisIsMyLabel.text = "" + health;
            if (healthBar != null)
                healthBar.SetHealth(health);
        }
    }
}