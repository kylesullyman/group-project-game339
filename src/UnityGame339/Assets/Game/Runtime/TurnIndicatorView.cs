using Game339.Shared.ViewModels;
using TMPro;
using UnityEngine;

namespace Game.Runtime
{
    public class TurnIndicatorView : ObserverMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI turnLabel;
        [SerializeField] private string playerTurnText = "Your Turn";
        [SerializeField] private string enemyTurnText = "Enemy Turn";

        private static ICombatViewModel CombatViewModel => ServiceResolver.Resolve<ICombatViewModel>();

        protected override void Subscribe()
        {
            CombatViewModel.IsPlayerTurn.ChangeEvent += OnTurnChanged;
            CombatViewModel.IsCombatOver.ChangeEvent += OnCombatOverChanged;
        }

        protected override void Unsubscribe()
        {
            CombatViewModel.IsPlayerTurn.ChangeEvent -= OnTurnChanged;
            CombatViewModel.IsCombatOver.ChangeEvent -= OnCombatOverChanged;
        }

        private void OnTurnChanged(bool isPlayerTurn)
        {
            if (turnLabel != null)
                turnLabel.text = isPlayerTurn ? playerTurnText : enemyTurnText;
        }

        private void OnCombatOverChanged(bool isOver)
        {
            if (turnLabel != null)
                turnLabel.gameObject.SetActive(!isOver);
        }
    }
}
