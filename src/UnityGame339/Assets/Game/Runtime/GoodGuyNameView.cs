using Game339.Shared.Models;
using TMPro;

namespace Game.Runtime
{
    public class GoodGuyNameView : ObserverMonoBehaviour
    {
        private static GameState GameState => ServiceResolver.Resolve<GameState>();

        public TextMeshProUGUI thisIsMyLabel;

        protected override void Subscribe()
        {
            GameState.GoodGuy.Name.ChangeEvent += OnGoodGuyNameChange;
            OnGoodGuyNameChange(GameState.GoodGuy.Name.Value);
        }

        protected override void Unsubscribe()
        {
            GameState.GoodGuy.Name.ChangeEvent -= OnGoodGuyNameChange;
        }

        private void OnGoodGuyNameChange(string newName)
        {
            if (thisIsMyLabel != null)
                thisIsMyLabel.text = newName;
        }
    }
}