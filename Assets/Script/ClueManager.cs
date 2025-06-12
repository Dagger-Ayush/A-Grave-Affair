using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClueManager : MonoBehaviour
{
    public static ClueManager Instance;

    public Transform clueBoxContainer;       
    public GameObject clueTextPrefab;       

    private HashSet<string> collectedClues = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddClue(string clueText)
    {
        if (collectedClues.Contains(clueText)) return;

        collectedClues.Add(clueText);

        GameObject newClue = Instantiate(clueTextPrefab, clueBoxContainer);
        newClue.GetComponent<TMP_Text>().text = clueText;
    }
}
