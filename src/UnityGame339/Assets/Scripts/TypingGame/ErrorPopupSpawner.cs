using TMPro;
using UnityEngine;

namespace Game.Runtime
{
    public class ErrorPopupSpawner : MonoBehaviour
    {
        public static ErrorPopupSpawner Instance;

        [SerializeField] private CanvasGroup errorPopupCanvasGroup;
        [SerializeField] private TextMeshProUGUI errorPopupText;
        [SerializeField] private AudioSource errorAudioSource;

        private Coroutine hideRoutine;

        private void Awake()
        {
            Instance = this;
            HideErrorPopup();
        }

        public void ShowErrorPopup(string message)
        {
            if (errorPopupText != null)
                errorPopupText.text = message;

            if (errorAudioSource != null)
                errorAudioSource.Play();

            if (errorPopupCanvasGroup != null)
            {
                errorPopupCanvasGroup.alpha = 1f;
                errorPopupCanvasGroup.interactable = true;
                errorPopupCanvasGroup.blocksRaycasts = true;
                if (hideRoutine != null)
                    StopCoroutine(hideRoutine);

                hideRoutine = StartCoroutine(AutoHide());
            }
        }

        public void HideErrorPopup()
        {
            if (errorPopupCanvasGroup != null)
            {
                errorPopupCanvasGroup.alpha = 0f;
                errorPopupCanvasGroup.interactable = false;
                errorPopupCanvasGroup.blocksRaycasts = false;
            }
        }
        
        private System.Collections.IEnumerator AutoHide()
        {
            yield return new WaitForSeconds(2f);
            HideErrorPopup();
        }
    }
}