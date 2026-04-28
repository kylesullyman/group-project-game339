using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordDatabase : MonoBehaviour
{
    public static WordDatabase Instance;

    private HashSet<string> validWords;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        LoadWords();
    }

    private void LoadWords()
    {
        TextAsset file = Resources.Load<TextAsset>("enable1");

        if (file == null)
        {
            Debug.LogError("enable1.txt not found in Resources folder!");
            return;
        }

        validWords = file.text
            .Split('\n')
            .Select(w => w.Trim().ToLower())
            .Where(w => w.Length > 1)
            .ToHashSet();

        Debug.Log($"Loaded {validWords.Count} words.");
    }

    public bool IsValidWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return false;

        return validWords.Contains(word.ToLower());
    }
}