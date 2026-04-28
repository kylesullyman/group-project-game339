using System.Collections;
using TMPro;
using UnityEngine;

public class DamagePopupEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation")]
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float moveSpeed = 60f;

    [Header("Scale Settings")]
    [SerializeField] private float baseScale = 1f;
    [SerializeField] private float scalePerDamage = 0.08f;
    [SerializeField] private float maxScale = 2.5f;

    [Header("Pop Animation")]
    [SerializeField] private float popMultiplier = 1.4f;
    [SerializeField] private float scaleDuration = 0.15f;

    private RectTransform rectTransform;

    private float finalScale = 1f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        StartCoroutine(Animate());
    }
    
    public void SetDamage(int damage)
    {
        finalScale = Mathf.Min(baseScale + damage * scalePerDamage, maxScale);
    }

    private IEnumerator Animate()
    {
        float time = 0f;

        float startScale = finalScale * popMultiplier;


        rectTransform.localScale = Vector3.one * startScale;


        while (time < scaleDuration)
        {
            time += Time.deltaTime;
            float t = time / scaleDuration;

            float scale = Mathf.Lerp(startScale, finalScale, t);
            rectTransform.localScale = Vector3.one * scale;

            yield return null;
        }

        rectTransform.localScale = Vector3.one * finalScale;
        
        time = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;

        while (time < lifetime)
        {
            time += Time.deltaTime;
            float t = time / lifetime;

            rectTransform.anchoredPosition = startPos + Vector2.up * (moveSpeed * time);

            if (canvasGroup != null)
                canvasGroup.alpha = 1f - t;

            yield return null;
        }

        Destroy(gameObject);
    }
}