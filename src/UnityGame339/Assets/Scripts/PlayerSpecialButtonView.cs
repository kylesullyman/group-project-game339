using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime
{
    public class PlayerSpecialButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;

        private void OnEnable()
        {
            if (button != null)
                button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            if (button != null)
                button.onClick.RemoveListener(OnClick);
        }

        private void Update()
        {
            if (button != null && CombatManager.Instance != null)
                button.interactable = CombatManager.Instance.IsCombatActive && CombatManager.Instance.IsPlayerTurn;
        }

        private void OnClick()
        {
            if (CombatManager.Instance != null)
                CombatManager.Instance.PlayerSpecial();
        }
    }
}