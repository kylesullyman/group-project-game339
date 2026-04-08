using TMPro;
using UnityEngine;

namespace Game.Runtime
{
    public class CombatResultUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI resultText;

        public void ShowWin()
        {
            if (panel != null)
                panel.SetActive(true);

            if (resultText != null)
                resultText.text = "You Win";
        }

        public void ShowLose()
        {
            if (panel != null)
                panel.SetActive(true);

            if (resultText != null)
                resultText.text = "You Lose";
        }

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }
}