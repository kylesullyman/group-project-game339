using UnityEngine;
using TMPro;

public class TypingAnimatorController : MonoBehaviour
{
    [Header("References")]
    public TMP_InputField inputField;
    public Animator animator;

    [Header("Animator Parameters")]
    public string typingBoolName = "IsTyping";
    public string typingSpeedFloatName = "TypingSpeed";
    public string mouseMoveBoolName = "MovingMouse"; // NEW

    [Header("Typing Settings")]
    public float idleDelay = 0.5f;

    [Header("Animation Speed")]
    public float baseTypingSpeed = 3f;
    public float maxTypingSpeed = 5f;
    public float speedPerCharacter = 0.15f;
    public float speedSmoothTime = 0.15f;

    [Header("Mouse Settings")]
    public float mouseIdleDelay = 0.1f; // how quickly it turns off

    private float lastTypeTime;
    private float currentAnimSpeed;
    private float speedVelocity;
    private bool isTyping;

    private int previousTextLength;

    // NEW mouse tracking
    private Vector3 lastMousePosition;
    private float lastMouseMoveTime;
    private bool isMouseMoving;

    private void Start()
    {
        currentAnimSpeed = baseTypingSpeed;
        previousTextLength = inputField != null ? inputField.text.Length : 0;

        animator.SetFloat(typingSpeedFloatName, baseTypingSpeed);
        animator.SetBool(typingBoolName, false);
        animator.SetBool(mouseMoveBoolName, false);

        lastMousePosition = Input.mousePosition;
    }

    private void Update()
    {
        if (inputField == null || animator == null)
            return;

        // ======================
        // TYPING LOGIC (unchanged)
        // ======================
        bool fieldFocused = inputField.isFocused;
        int currentTextLength = inputField.text.Length;

        bool textChanged = currentTextLength != previousTextLength;

        if (fieldFocused && textChanged)
        {
            lastTypeTime = Time.time;

            if (!isTyping)
            {
                isTyping = true;
                animator.SetBool(typingBoolName, true);
            }

            int characterDifference = Mathf.Abs(currentTextLength - previousTextLength);

            float targetSpeed = baseTypingSpeed + characterDifference * speedPerCharacter;
            targetSpeed = Mathf.Clamp(targetSpeed, baseTypingSpeed, maxTypingSpeed);

            currentAnimSpeed = Mathf.SmoothDamp(
                currentAnimSpeed,
                targetSpeed,
                ref speedVelocity,
                speedSmoothTime
            );

            animator.SetFloat(typingSpeedFloatName, currentAnimSpeed);
        }

        if (isTyping && Time.time - lastTypeTime > idleDelay)
        {
            isTyping = false;
            animator.SetBool(typingBoolName, false);

            currentAnimSpeed = baseTypingSpeed;
            animator.SetFloat(typingSpeedFloatName, baseTypingSpeed);
        }

        previousTextLength = currentTextLength;

        // ======================
        // MOUSE MOVEMENT LOGIC (NEW)
        // ======================
        Vector3 currentMousePosition = Input.mousePosition;

        if (currentMousePosition != lastMousePosition)
        {
            lastMouseMoveTime = Time.time;

            if (!isMouseMoving)
            {
                isMouseMoving = true;
                animator.SetBool(mouseMoveBoolName, true);
            }
        }

        if (isMouseMoving && Time.time - lastMouseMoveTime > mouseIdleDelay)
        {
            isMouseMoving = false;
            animator.SetBool(mouseMoveBoolName, false);
        }

        lastMousePosition = currentMousePosition;
    }
}