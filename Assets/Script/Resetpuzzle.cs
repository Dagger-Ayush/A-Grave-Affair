using System.Collections.Generic;
using UnityEngine;

public class Resetpuzzle : MonoBehaviour
{
    public List<PuzzleData> allPuzzles;
    public void ResetPuzzleCompletion()
    {
        foreach (PuzzleData puzzle in allPuzzles)
        {
            puzzle.isCompleted = false;
        }
    }
}
