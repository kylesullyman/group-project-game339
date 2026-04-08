using Game339.Shared.Models;
using TMPro;

namespace Game.Runtime
{
    public class BadGuyNameView : ObserverMonoBehaviour
    {
        private static GameState GameState => ServiceResolver.Resolve<GameState>();

        public TextMeshProUGUI thisIsMyLabel;

        protected override void Subscribe()
        {
            GameState.BadGuy.Name.ChangeEvent += OnBadGuyNameChange;
            OnBadGuyNameChange(GameState.BadGuy.Name.Value);
        }

        protected override void Unsubscribe()
        {
            GameState.BadGuy.Name.ChangeEvent -= OnBadGuyNameChange;
        }

        private void OnBadGuyNameChange(string newName)
        {
            if (thisIsMyLabel != null)
                thisIsMyLabel.text = newName;
        }
    }
}