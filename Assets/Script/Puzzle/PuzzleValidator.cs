using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ObjectPickHandler;

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

    [Header ("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip allCorrectClip;
    [SerializeField] private AudioClip twoOrLessIncorrectClip;
    [SerializeField] private AudioClip allIncorrectClip;
   
    private void Start()
    {
        ResetPuzzleCompletion();

        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();

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
            PlayClipSafe(allCorrectClip);

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
            //if(progression != null && currentPuzzle.puzzleID == 1)
            //{
            //    progression.OnPuzzle1Solved();
            //    Debug.LogWarning("working");
            //}

            if(progression != null)
            {
                if (currentPuzzle.puzzleID == 1)
                {
                    progression.OnPuzzle1Solved();
                    Debug.Log("Puzzle 1 completed, progression triggered.");
                }
                else if (currentPuzzle.puzzleID == 2)
                {
                    progression.OnPuzzle2Solved();
                    Debug.Log("Puzzle 2 completed, progression triggered.");
                }
                else if (currentPuzzle.puzzleID == 3)
                {
                    progression.OnPuzzle3Solved();
                    Debug.Log("Puzzle 3 completed, progression triggered.");
                }
                else if (currentPuzzle.puzzleID == 5)
                {
                    progression.OnPuzzle5Solved();
                    Debug.Log("Puzzle 5 completed, progression triggered.");
                }
                else if (currentPuzzle.puzzleID == 6)
                {
                    progression.OnPuzzle6Solved();
                    Debug.Log("Puzzle 6 completed, progression triggered.");
                }
                else if (currentPuzzle.puzzleID == 7)
                {
                    progression.OnPuzzle7Solved();
                    Debug.Log("Puzzle 7 completed, progression triggered.");
                }
            }

        }
        else if (incorrectCount <= 2)
        {
            PlayClipSafe(twoOrLessIncorrectClip);

            feedbackText.text = "2 or fewer clues are incorrect.";
            Debug.Log($"{incorrectCount} clue{(incorrectCount > 1 ? "s" : "")} incorrect.");
        }
        else
        {
            PlayClipSafe(allIncorrectClip);

            feedbackText.text = "The sentence is filled incorrectly.";
            Debug.Log("The sentence is filled incorrectly.");
        }
    }

    private void PlayClipSafe(AudioClip clip)
    {
        if(clip == null) 
            return;

        if(audioSource != null)
        {
            //audioSource.PlayOneShot(clip);
        }
        else
        {
           // Debug.LogError("No audio Source");
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

                    if (objectPickHandler.type == InspectType.Letter_1)
                    {
                        objectPickHandler.enabled = true;
                        objectPickHandler.shouldWork = true;
                    }
                    if (objectPickHandler.type == InspectType.TutorialLetter)
                    {
                        objectPickHandler.enabled = true;
                        objectPickHandler.shouldWork = true;
                    }

                }
            }

        }
    }
}

