using System.Collections.Generic;

public static class GlobalClueInventory
{
    private static readonly HashSet<string> collectedClues = new();

    public static void AddClue(string clue)
    {
        collectedClues.Add(clue);
    }

    public static bool HasClue(string clue)
    {
        return collectedClues.Contains(clue);
    }

    public static IEnumerable<string> GetAllClues()
    {
        return collectedClues;
    }

    public static void RemoveClues(IEnumerable<string> clues)
    {
        foreach (string clue in clues)
        {
            collectedClues.Remove(clue);
        }
    }
}
