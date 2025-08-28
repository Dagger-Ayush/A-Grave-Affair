using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleValidator : MonoBehaviour
{
    public PuzzleData currentPuzzle;
    public List<DropZone> dropZones;
    public TabletManager tabletManager;
    public TMP_Text feedbackText;
    public List<PuzzleData> allPuzzles;

    [Header ("Extra References")]
    public static int CorrectFilledCount = 0;//Checking for if the entier Puzzle is filled correct
    bool isIncreased;
   
    private void Start()
    {
        ResetPuzzleCompletion();
    }
    private void Update()
    {
       
        if (currentPuzzle != null && currentPuzzle.isCompleted && !isIncreased)
        {
            CorrectFilledCount++;
            isIncreased = true;
        }
        LetterEnabling();

    }
    
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.V))
    //    {
    //        Debug.Log("Manual ValidatePuzzle check");
    //        ValidatePuzzle();
    //    }
    //}
    public bool ValidatePuzzle()
    {
        if (dropZones.Count != currentPuzzle.correctAnswers.Count)
        {
            Debug.LogError($"Mismatch: dropZones.Count = {dropZones.Count}, correctAnswers.Count = {currentPuzzle.correctAnswers.Count}");
            return false;
        }
        for (int i = 0; i < dropZones.Count; i++)
        {
            var clue = dropZones[i].currentClue;

            if (clue == null)
            {
                Debug.Log($"Zone {i}: clue = null, expected = {currentPuzzle.correctAnswers[i]}");
                return false;
            }
            string placed = clue.clueText.Trim().ToLower().Normalize();
            string correct = currentPuzzle.correctAnswers[i].Trim().ToLower().Normalize();
            Debug.Log($"Zone {i}: clue = {placed}, expected = {correct}");

            if (placed != correct )
                return false;
        }
        return true;
    }

    public void CheckAllDropZonesFilled()
    {
        Debug.Log("Validating puzzle: " + currentPuzzle.name);
        dropZones = dropZones.Where(dz => dz != null).ToList();

        foreach (var zone in dropZones)
        {
            if (zone.currentClue == null)
            {
                feedbackText.text = "Please place all clues.";
                return;
            }
        }

        int incorrectCount = 0;

        for (int i = 0; i < dropZones.Count; i++)
        {
            var clue = dropZones[i].currentClue;
            string placed = clue.clueText.Trim().ToLower().Normalize();
            string correct = currentPuzzle.correctAnswers[i].Trim().ToLower().Normalize();

            if (clue == null || placed != correct)
                incorrectCount++;
        }

        if (incorrectCount == 0)
        {
            isIncreased = false;
            feedbackText.text = "Sentence is filled correctly.";
            Debug.Log("Puzzle is Correct");

            currentPuzzle.isCompleted = true;

            if (tabletManager != null)
            {
                tabletManager.DisplayCompletedSentence(currentPuzzle);
            }

            string fullSentence = currentPuzzle.sentenceTemplate;

            foreach (var dz in dropZones)
            {
                string clueText = dz.currentClue != null ? dz.currentClue.clueText : "_";
                int index = fullSentence.IndexOf("_");
                if (index >= 0)
                {
                    fullSentence = fullSentence.Substring(0, index) + clueText + fullSentence.Substring(index + 1);
                }
            }

            List<string> usedClues = dropZones.Select(dz => dz.currentClue.clueText).ToList();

            // Remove from clue box
            ClueManager.Instance.RemoveUsedClues(usedClues);

            var progression = FindAnyObjectByType<PuzzleProgression>();
            if(progression != null && currentPuzzle.puzzleID == 1)
            {
                progression.OnPuzzle1Solved();
                Debug.LogWarning("working");
            }

        }
        else if (incorrectCount <= 2)
        {
            feedbackText.text = "2 or fewer clues are incorrect.";
            Debug.Log($"{incorrectCount} clue{(incorrectCount > 1 ? "s" : "")} incorrect.");
        }
        else
        {
            feedbackText.text = "The sentence is filled incorrectly.";
            Debug.Log("The sentence is filled incorrectly.");
        }
    }
   
    public void ClearDropZones()
    {
        DropZone[] dropZones = Object.FindObjectsByType<DropZone>(FindObjectsSortMode.None);
        foreach(DropZone dz in dropZones)
        {
            Destroy(dz.gameObject);
        }

    }
    public void ResetPuzzleCompletion()
    {
        foreach (PuzzleData puzzle in allPuzzles)
        {
            puzzle.isCompleted = false;
        }
    }
    private void LetterEnabling()
    {

        if (CorrectFilledCount >= 2)
        {
           
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, 100);

            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out ObjectPickHandler objectPickHandler))
                {

                    if (objectPickHandler.isLetter_1)
                    {
                        objectPickHandler.shouldWork = true;
                    }
                    if (objectPickHandler.isLetter_2)
                    {
                     
                        objectPickHandler.shouldWork = true;
                    }

                }
            }

        }
    }
}

