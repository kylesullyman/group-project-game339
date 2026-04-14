using Game339.Shared.ViewModels;
using TMPro;

namespace Game.Runtime
{
    public class StatusMessageView : ObserverMonoBehaviour
    {
        public TextMeshProUGUI messageLabel;

        private static ICombatViewModel CombatViewModel => ServiceResolver.Resolve<ICombatViewModel>();

        protected override void Subscribe()
        {
            CombatViewModel.StatusMessage.ChangeEvent += OnStatusMessageChanged;
        }

        protected override void Unsubscribe()
        {
            CombatViewModel.StatusMessage.ChangeEvent -= OnStatusMessageChanged;
        }

        private void OnStatusMessageChanged(string message)
        {
            if (messageLabel != null)
                messageLabel.text = message;
        }
    }
}
