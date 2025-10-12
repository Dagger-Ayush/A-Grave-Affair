using UnityEngine;

public class ClueDebug : MonoBehaviour
{
    public string clueToAdd;

    [ContextMenu("Add Clue")]
    public void AddClue()
    {
        GlobalClueInventory.AddClue(clueToAdd);
        Debug.Log($"Clue added: {clueToAdd}");
    }

    [ContextMenu("Show All Clues")]
    public void ShowClues()
    {
        foreach (var clue in GlobalClueInventory.GetAllClues())
        {
            Debug.Log(clue);
        }
    }
}