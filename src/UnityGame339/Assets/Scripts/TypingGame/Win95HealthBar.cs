using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Win95HealthBar : MonoBehaviour
{
    [SerializeField] private Transform barParent;
    [SerializeField] private Image barPrefab;

    private List<Image> bars = new List<Image>();

    private int maxHealth = 100;
    private int healthPerBar = 5;

    private void Start()
    {
        HorizontalLayoutGroup layout = barParent.GetComponent<HorizontalLayoutGroup>();
        if (layout == null)
        {
            layout = barParent.gameObject.AddComponent<HorizontalLayoutGroup>();
        }

        layout.spacing = 2f;
        layout.padding = new RectOffset(2, 2, 2, 2);
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        int barCount = maxHealth / healthPerBar;

        for (int i = 0; i < barCount; i++)
        {
            Image newBar = Instantiate(barPrefab, barParent);
            newBar.gameObject.SetActive(true);
            bars.Add(newBar);
        }

        barPrefab.gameObject.SetActive(false);

        SetHealth(maxHealth);
    }

    public void SetHealth(int currentHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        int barsToShow = Mathf.CeilToInt(currentHealth / (float)healthPerBar);

        for (int i = 0; i < bars.Count; i++)
        {
            bars[i].enabled = i < barsToShow;
        }
    }
}