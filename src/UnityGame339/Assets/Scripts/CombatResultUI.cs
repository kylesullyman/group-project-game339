using Game.Runtime;
using Game339.Shared.ViewModels;
using TMPro;
using UnityEngine;

public class CombatResultUI : ObserverMonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI resultText;

    private static ICombatViewModel CombatViewModel => ServiceResolver.Resolve<ICombatViewModel>();

    protected override void Subscribe()
    {
        CombatViewModel.IsCombatOver.ChangeEvent += OnCombatOverChanged;
        CombatViewModel.PlayerWon.ChangeEvent += OnPlayerWonChanged;
    }

    protected override void Unsubscribe()
    {
        CombatViewModel.IsCombatOver.ChangeEvent -= OnCombatOverChanged;
        CombatViewModel.PlayerWon.ChangeEvent -= OnPlayerWonChanged;
    }

    private void OnCombatOverChanged(bool isOver)
    {
        if (panel != null)
            panel.SetActive(isOver);
    }

    private void OnPlayerWonChanged(bool playerWon)
    {
        if (resultText != null)
            resultText.text = playerWon ? "You Win" : "You Lose";
    }
}
