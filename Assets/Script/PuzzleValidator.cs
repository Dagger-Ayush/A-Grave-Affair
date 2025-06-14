using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleValidator : MonoBehaviour
{
    public PuzzleData currentPuzzle;
    public List<DropZone> dropZones;
    public TabletManager tabletManager;
    public TMP_Text feedbackText;


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
            feedbackText.text = "Sentence is filled correctly.";
            Debug.Log("Puzzle is Correct");
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
}

