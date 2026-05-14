using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private Transform titleText;
    [SerializeField] private Vector3 textPosition = new Vector3(0f, 90f, 0f);

    [Header("Start Button")]
    [SerializeField] private Button startButton;

    [Header("Cameras")]
    [SerializeField] private Camera startScreenCamera;
    [SerializeField] private Camera mainCamera;
    
    [Header("Input")]
    [SerializeField] private TMP_Text textInputField;

    public void Start()
    {
        if (startScreenCamera != null)
            startScreenCamera.enabled = true;
        if (mainCamera != null)
            mainCamera.enabled = false;
        if (textInputField != null)
            textInputField.text = "Hit Start to Play!";

        if (startButton != null)
            startButton.onClick.AddListener(OnStartPressed);
    }

    public void Initialize()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartPressed);
    }

    private void OnStartPressed()
    {
        if (mainCamera != null)
            mainCamera.enabled = true;
        if (startScreenCamera != null)
            startScreenCamera.enabled = false;
        if (startButton != null)
            startButton.enabled = false;
        if (textInputField != null)
            textInputField.text = "Enter text: ";
        gameObject.SetActive(false);
        
        
    }
}