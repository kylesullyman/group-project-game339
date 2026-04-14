using Game339.Shared.ViewModels;
using TMPro;

namespace Game.Runtime
{
    public class GoodGuyNameView : ObserverMonoBehaviour
    {
        public TextMeshProUGUI thisIsMyLabel;

        private static ICombatViewModel CombatViewModel => ServiceResolver.Resolve<ICombatViewModel>();

        protected override void Subscribe()
        {
            CombatViewModel.PlayerHealthBar.Name.ChangeEvent += OnNameChanged;
        }

        protected override void Unsubscribe()
        {
            CombatViewModel.PlayerHealthBar.Name.ChangeEvent -= OnNameChanged;
        }

        private void OnNameChanged(string playerName)
        {
            if (thisIsMyLabel != null)
                thisIsMyLabel.text = playerName;
        }
    }
}
