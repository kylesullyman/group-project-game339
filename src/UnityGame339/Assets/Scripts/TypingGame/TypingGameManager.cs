using System.Collections;
using Game339.Shared.Diagnostics;
using Game339.Shared.Models;
using Game339.Shared.Services;
using Game339.Shared.ViewModels;
using TMPro;
using UnityEngine;

namespace Game.Runtime
{
    public class MainGameManager : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_InputField wordInputField;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private TextMeshProUGUI submittedWordText;
        [SerializeField] private TextMeshProUGUI ruleText;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Fade")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private float fadeDuration = 0.3f;

        [Header("Game Settings")]
        [SerializeField] private float turnTransitionDelay = 2f;
        [SerializeField] private int startingHealth = 100;
        [SerializeField] private int minDamage = 1;
        [SerializeField] private int maxDamage = 12;

        [Header("Turn Timer Settings")]
        [SerializeField] private float startingTurnTime = 10f;
        [SerializeField] private float minimumTurnTime = 3f;
        [SerializeField] private float timerDecreasePerRound = 1f;

        private int currentPlayerTurn = 1;
        private bool canType = true;
        private bool isTransitioning = false;
        private bool gameEnded = false;

        private string lastWord = "";
        private char requiredStartingLetter;
        private bool hasRequiredLetter = false;

        private float currentTurnTime;
        private float turnTimeRemaining;
        private Coroutine turnTimerRoutine;

        private int turnsTakenThisRound = 0;

        private static GameState GameState => ServiceResolver.Resolve<GameState>();
        private static IDamageService DamageSvc => ServiceResolver.Resolve<IDamageService>();
        private static ICombatViewModel CombatViewModel => ServiceResolver.Resolve<ICombatViewModel>();
        private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();

        private void Start()
        {
            if (wordInputField != null)
                wordInputField.onSubmit.AddListener(HandleSubmit);

            StartTypingBattle();
            StartTurn();
        }

        private void OnDestroy()
        {
            if (wordInputField != null)
                wordInputField.onSubmit.RemoveListener(HandleSubmit);
        }

        private void StartTypingBattle()
        {
            gameEnded = false;
            currentPlayerTurn = 1;
            turnsTakenThisRound = 0;

            lastWord = "";
            hasRequiredLetter = false;

            currentTurnTime = startingTurnTime;

            GameState.GoodGuy.Health.Value = startingHealth;
            GameState.BadGuy.Health.Value = startingHealth;

            CombatViewModel.OnCombatStarted(startingHealth, startingHealth);
            CombatViewModel.OnStatusUpdated("Typing battle started.");
            Log.Info("Typing battle started.");
        }

        private void StartTurn()
        {
            if (gameEnded) return;

            canType = true;
            isTransitioning = false;
            turnTimeRemaining = currentTurnTime;

            if (loadingScreen != null)
                loadingScreen.SetActive(false);

            if (turnText != null)
                turnText.text = "Player " + currentPlayerTurn + "'s Turn";

            if (errorText != null)
                errorText.text = "";

            if (ruleText != null)
            {
                if (hasRequiredLetter)
                    ruleText.text = "Start with: " + char.ToUpper(requiredStartingLetter);
                else
                    ruleText.text = "Type any word";
            }

            if (wordInputField != null)
            {
                wordInputField.interactable = true;
                wordInputField.text = "";
                wordInputField.ActivateInputField();
                wordInputField.Select();
            }

            UpdateTimerText();

            if (turnTimerRoutine != null)
                StopCoroutine(turnTimerRoutine);

            turnTimerRoutine = StartCoroutine(RunTurnTimer());

            if (currentPlayerTurn == 1)
                CombatViewModel.OnPlayerTurnBegan();
            else
                CombatViewModel.OnEnemyTurnBegan();
        }

        private IEnumerator RunTurnTimer()
        {
            while (turnTimeRemaining > 0f && canType && !isTransitioning && !gameEnded)
            {
                turnTimeRemaining -= Time.deltaTime;
                UpdateTimerText();
                yield return null;
            }

            if (!canType || isTransitioning || gameEnded)
                yield break;

            HandleTurnTimeout();
        }

        private void UpdateTimerText()
        {
            if (timerText != null)
                timerText.text = "Time: " + Mathf.CeilToInt(turnTimeRemaining);
        }

        private void HandleTurnTimeout()
        {
            canType = false;
            isTransitioning = true;

            if (wordInputField != null)
                wordInputField.interactable = false;

            if (submittedWordText != null)
                submittedWordText.text = "Player " + currentPlayerTurn + " ran out of time!";

            CombatViewModel.OnStatusUpdated("Player " + currentPlayerTurn + " ran out of time.");
            Log.Info("Timeout");

            StartCoroutine(SwitchTurnAfterDelay());
        }

        private void HandleSubmit(string submittedText)
        {
            if (!canType || isTransitioning || gameEnded)
                return;

            string typedWord = submittedText.Trim().ToLower();

            if (string.IsNullOrEmpty(typedWord))
                return;

            if (!IsValidWord(typedWord))
                return;

            SubmitWord(typedWord);
        }

        private bool IsValidWord(string word)
        {
            if (!hasRequiredLetter)
                return true;

            if (word[0] != requiredStartingLetter)
            {
                if (errorText != null)
                    errorText.text = "Must start with '" + char.ToUpper(requiredStartingLetter) + "'";

                return false;
            }

            return true;
        }

        private void SubmitWord(string word)
        {
            canType = false;
            isTransitioning = true;

            if (turnTimerRoutine != null)
                StopCoroutine(turnTimerRoutine);

            lastWord = word;
            requiredStartingLetter = lastWord[lastWord.Length - 1];
            hasRequiredLetter = true;

            int damage = Mathf.Clamp(word.Length, minDamage, maxDamage);

            if (currentPlayerTurn == 1)
                DamageSvc.ApplyDamage(GameState.BadGuy, damage);
            else
                DamageSvc.ApplyDamage(GameState.GoodGuy, damage);

            if (submittedWordText != null)
                submittedWordText.text = $"Player {currentPlayerTurn}: {word} ({damage} dmg)";

            if (CheckForGameEnd())
                return;

            StartCoroutine(SwitchTurnAfterDelay());
        }

        private bool CheckForGameEnd()
        {
            if (GameState.GoodGuy.Health.Value <= 0)
            {
                EndGame(2);
                return true;
            }

            if (GameState.BadGuy.Health.Value <= 0)
            {
                EndGame(1);
                return true;
            }

            return false;
        }

        private IEnumerator SwitchTurnAfterDelay()
        {
            if (loadingScreen != null)
                loadingScreen.SetActive(true);

            yield return StartCoroutine(Fade(1f));

            yield return new WaitForSeconds(turnTransitionDelay);

            AdvanceRoundTimer();

            currentPlayerTurn = currentPlayerTurn == 1 ? 2 : 1;

            yield return StartCoroutine(Fade(0f));

            StartTurn();
        }

        private void AdvanceRoundTimer()
        {
            turnsTakenThisRound++;

            if (turnsTakenThisRound >= 2)
            {
                turnsTakenThisRound = 0;
                currentTurnTime = Mathf.Max(minimumTurnTime, currentTurnTime - timerDecreasePerRound);

                CombatViewModel.OnStatusUpdated("Timer now: " + currentTurnTime);
            }
        }

        private IEnumerator Fade(float targetAlpha)
        {
            if (fadeCanvasGroup == null)
                yield break;

            float start = fadeCanvasGroup.alpha;
            float time = 0f;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                float t = time / fadeDuration;
                fadeCanvasGroup.alpha = Mathf.Lerp(start, targetAlpha, t);
                yield return null;
            }

            fadeCanvasGroup.alpha = targetAlpha;
            fadeCanvasGroup.blocksRaycasts = targetAlpha > 0.5f;
        }

        private void EndGame(int winner)
        {
            gameEnded = true;
            canType = false;

            if (turnTimerRoutine != null)
                StopCoroutine(turnTimerRoutine);

            if (turnText != null)
                turnText.text = $"Player {winner} Wins!";

            if (timerText != null)
                timerText.text = "";

            CombatViewModel.OnCombatEnded(winner == 1);
        }
    }
}