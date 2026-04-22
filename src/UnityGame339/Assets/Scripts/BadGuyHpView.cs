using Game339.Shared.Models;
using TMPro;

namespace Game.Runtime
{
    public class BadGuyHpView : ObserverMonoBehaviour
    {
        private static GameState GameState => ServiceResolver.Resolve<GameState>();

        public TextMeshProUGUI thisIsMyLabel;

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
                thisIsMyLabel.text = "Player 2 Health: " + health;
        }
    }
}