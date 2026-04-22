using System.Collections;
using TMPro;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField wordInputField;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI submittedWordText;
    [SerializeField] private TextMeshProUGUI ruleText;
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("Settings")]
    [SerializeField] private float turnTransitionDelay = 2f;

    private int currentPlayerTurn = 1;
    private bool canType = true;
    private bool isTransitioning = false;

    private string lastWord = "";
    private char requiredStartingLetter;
    private bool hasRequiredLetter = false;

    private void Start()
    {
        if (wordInputField != null)
            wordInputField.onSubmit.AddListener(HandleSubmit);

        StartTurn();
    }

    private void OnDestroy()
    {
        if (wordInputField != null)
            wordInputField.onSubmit.RemoveListener(HandleSubmit);
    }

    private void StartTurn()
    {
        canType = true;
        isTransitioning = false;

        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        if (turnText != null)
            turnText.text = "Player " + currentPlayerTurn + "'s Turn";

        if (errorText != null)
            errorText.text = "";

        if (ruleText != null)
        {
            if (hasRequiredLetter)
                ruleText.text = "Type a word starting with: " + char.ToUpper(requiredStartingLetter);
            else
                ruleText.text = "Type any starting word";
        }

        if (wordInputField != null)
        {
            wordInputField.text = "";
            wordInputField.interactable = true;
            wordInputField.ActivateInputField();
            wordInputField.Select();
        }
    }

    private void HandleSubmit(string submittedText)
    {
        if (!canType || isTransitioning)
            return;

        string typedWord = submittedText.Trim().ToLower();

        if (string.IsNullOrEmpty(typedWord))
            return;

        if (!IsValidWord(typedWord))
            return;

        SubmitWord(typedWord);
    }

    private bool IsValidWord(string typedWord)
    {
        if (!hasRequiredLetter)
            return true;

        if (typedWord[0] != requiredStartingLetter)
        {
            if (errorText != null)
                errorText.text = "Word must start with '" + char.ToUpper(requiredStartingLetter) + "'";

            if (wordInputField != null)
            {
                wordInputField.ActivateInputField();
                wordInputField.Select();
            }

            return false;
        }

        return true;
    }

    private void SubmitWord(string typedWord)
    {
        canType = false;
        isTransitioning = true;

        lastWord = typedWord;
        requiredStartingLetter = lastWord[lastWord.Length - 1];
        hasRequiredLetter = true;

        if (wordInputField != null)
            wordInputField.interactable = false;

        if (submittedWordText != null)
            submittedWordText.text = "Player " + currentPlayerTurn + " typed: " + typedWord;

        StartCoroutine(SwitchTurnAfterDelay());
    }

    private IEnumerator SwitchTurnAfterDelay()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        yield return new WaitForSeconds(turnTransitionDelay);

        currentPlayerTurn = (currentPlayerTurn == 1) ? 2 : 1;

        StartTurn();
    }
}