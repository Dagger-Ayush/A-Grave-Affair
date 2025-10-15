using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalPuzzleStateData", menuName = "Game/Global Puzzle State Data")]
public class GlobalPuzzleStateData : ScriptableObject
{
    public List<int> completedPuzzles = new List<int>();

    public void MarkComplete(int puzzleId)
    {
        if(!completedPuzzles.Contains(puzzleId))
            completedPuzzles.Add(puzzleId);
    }
    public bool IsComplete(int puzzleId)
    {
        return completedPuzzles.Contains(puzzleId);
    }

    public void ResetAll()
    {
        completedPuzzles.Clear();
    }
}
