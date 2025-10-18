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

    public AudioSource audioSource;
    public AudioClip clueAddSound;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();

        collectedClues = GlobalClueInventory.GetAllClues().ToList();
    }

    public void AddClue(string clueText)
    {
        //if (collectedClues.Contains(clueText)) return;
        if (GlobalClueInventory.HasClue(clueText))
            return;

        ObjectPickHandler.clueCount++;
        ObjectInteract.clueCount++;

        GlobalClueInventory.AddClue(clueText);
        collectedClues.Add(clueText);

        if (clueAddSound != null && audioSource != null)
            audioSource.PlayOneShot(clueAddSound);
    }
    public bool ClueCheck(string clueText)
    {
        return GlobalClueInventory.HasClue(clueText);
        //if (collectedClues.Contains(clueText))
        //{
        //return true ;
        //}
        //else
        //{
        //    return false;
        //}
    }
    public void ShowRelevantClue(PuzzleData currentPuzzle)
    {
        if (clueBox == null || clueTextPrefab == null || clueBoxContainer == null)
            return;

        
        for (int i = clueBoxContainer.childCount - 1; i >= 0; i--)  // Clear old clues safely
        {
            Destroy(clueBoxContainer.GetChild(i).gameObject);
            //Transform child = clueBoxContainer.GetChild(i);
            //if (child != null)
            //    Destroy(child.gameObject);
        }

        
        foreach (string clue in currentPuzzle.requiredClues) // Show only clues relevant to the current puzzle
        {
            //if (collectedClues.Contains(clue))
            //{
            //    GameObject clueGO = Instantiate(clueTextPrefab, clueBoxContainer);
            //    TMP_Text clueText = clueGO.GetComponentInChildren<TMP_Text>();
            //    if (clueText != null)
            //        clueText.text = clue;

            //    DraggableClue draggable = clueGO.GetComponent<DraggableClue>();
            //    if (draggable != null)
            //        draggable.clueText = clue;
            //}
            if (GlobalClueInventory.HasClue(clue))
            {
                GameObject clueGO = Instantiate(clueTextPrefab, clueBoxContainer);
                TMP_Text clueText = clueGO.GetComponentInChildren<TMP_Text>();
                if (clueText != null)
                    clueText.text = clue;

                DraggableClue draggable = clueGO.GetComponent<DraggableClue>();
                if (draggable != null)
                    draggable.clueText = clue;
            }
        }

        if (clueBox != null && !clueBox.activeSelf)
            clueBox.SetActive(true);
    }
    public void RemoveUsedClues(List<string> clues)
    {
        GlobalClueInventory.RemoveClues(clues);

        // Also update the local list so UI stays accurate
        collectedClues = GlobalClueInventory.GetAllClues().ToList();
        //foreach (string clue in clues)
        //{
        //    string normalized = clue.Trim().ToLower();
        //    collectedClues.RemoveAll(c => c.Trim().ToLower() == normalized);
        //}
    }
   
}
