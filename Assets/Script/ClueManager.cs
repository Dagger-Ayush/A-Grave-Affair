using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ClueManager : MonoBehaviour
{
    public static ClueManager Instance;

    public Transform clueBoxContainer;       
    public GameObject clueTextPrefab;

    public GameObject clueBox;

    public List<string> collectedClues = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddClue(string clueText)
    {
        if (collectedClues.Contains(clueText)) return;

        collectedClues.Add(clueText);
    }
    public bool ClueCheck(string clueText)
    {
        if (collectedClues.Contains(clueText))
        {
        return true ;
        }
        else
        {
            return false;
        }
    }
    public void ShowRelevantClue(PuzzleData currentPuzzle)
    {
        if (clueBox == null || clueTextPrefab == null || clueBoxContainer == null)
            return;

        
        for (int i = clueBoxContainer.childCount - 1; i >= 0; i--)  // Clear old clues safely
        {
            Transform child = clueBoxContainer.GetChild(i);
            if (child != null)
                Destroy(child.gameObject);
        }

        
        foreach (string clue in currentPuzzle.requiredClues) // Show only clues relevant to the current puzzle
        {
            if (collectedClues.Contains(clue))
            {
                GameObject clueGO = Instantiate(clueTextPrefab, clueBoxContainer);
                TMP_Text clueText = clueGO.GetComponentInChildren<TMP_Text>();
                if (clueText != null)
                    clueText.text = clue;
            }
        }

        if (clueBox != null && !clueBox.activeSelf)
            clueBox.SetActive(true);
    }
    public void RemoveUsedClues(List<string> clues)
    {
        foreach (string clue in clues)
        {
            string normalized = clue.Trim().ToLower();
            collectedClues.RemoveAll(c => c.Trim().ToLower() == normalized);
        }
    }
}
