using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private Transform titleText;
    [SerializeField] private Vector3 textPosition = new Vector3(0f, 90f, 0f);

    [Header("Start Button")]
    [SerializeField] private Button startButton;

    [Header("References")]
    [SerializeField] private BoardManager boardManager;

    public void Initialize()
    {
        if (titleText != null)
            titleText.localPosition = textPosition;

        if (startButton != null)
            startButton.onClick.AddListener(OnStartPressed);
    }

    private void OnStartPressed()
    {
        if (boardManager != null)
            boardManager.GenerateBoard();

        gameObject.SetActive(false);
    }
}