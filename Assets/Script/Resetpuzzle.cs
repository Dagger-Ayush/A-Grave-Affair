using System.Collections.Generic;
using UnityEngine;

public class Resetpuzzle : MonoBehaviour
{
    public List<PuzzleData> allPuzzles;

    public void ResetPuzzleCompletion()
    {
        // Reset all puzzles
        foreach (PuzzleData puzzle in allPuzzles)
        {
            puzzle.isCompleted = false;
        }

        // Reset dress interaction PlayerPref
        if (PlayerPrefs.HasKey("DressInteractionDone"))
        {
            PlayerPrefs.DeleteKey("DressInteractionDone");
        }

        // Reset outside door PlayerPref
        if (PlayerPrefs.HasKey("EnteredOutsideDoor"))
        {
            PlayerPrefs.DeleteKey("EnteredOutsideDoor");
        }

        // Save changes immediately
        PlayerPrefs.Save();

        Debug.Log("All puzzles, dress interaction, and outside door reset!");
    }
}
