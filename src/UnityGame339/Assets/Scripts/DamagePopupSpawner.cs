using TMPro;
using UnityEngine;

public class DamagePopupSpawner : MonoBehaviour
{
    public static DamagePopupSpawner Instance;

    [Header("Popup")]
    [SerializeField] private GameObject damagePopupPrefab;

    [Header("Canvas")]
    [SerializeField] private RectTransform canvasTransform;

    [Header("Spawn Points (UI)")]
    [SerializeField] private RectTransform player1DamagePoint;
    [SerializeField] private RectTransform player2DamagePoint;

    [Header("Random Offset")]
    [SerializeField] private float randomOffsetRange = 2f;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnDamagePopup(int playerNumber, int damageAmount)
    {
        if (damagePopupPrefab == null || canvasTransform == null)
        {
            Debug.LogWarning("Missing prefab or canvas.");
            return;
        }

        RectTransform spawnPoint = playerNumber == 1 ? player1DamagePoint : player2DamagePoint;

        if (spawnPoint == null)
        {
            Debug.LogWarning("Missing spawn point.");
            return;
        }

 
        GameObject popup = Instantiate(damagePopupPrefab, canvasTransform);

        RectTransform popupRect = popup.GetComponent<RectTransform>();


        popupRect.anchoredPosition = spawnPoint.anchoredPosition;


        popupRect.anchoredPosition += new Vector2(
            Random.Range(-randomOffsetRange, randomOffsetRange),
            Random.Range(-randomOffsetRange, randomOffsetRange)
        );

        // Set text
        TextMeshProUGUI text = popup.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = "-" + damageAmount;
        }
        else
        {
            Debug.LogWarning("No TextMeshProUGUI found on popup.");
        }
        DamagePopupEffect effect = popup.GetComponent<DamagePopupEffect>();
        if (effect != null)
        {
            effect.SetDamage(damageAmount);
        }
    }
}